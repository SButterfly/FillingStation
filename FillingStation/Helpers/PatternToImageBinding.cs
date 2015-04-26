using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FillingStation.Annotations;
using FillingStation.Core.Patterns;
using FillingStation.Core.Properties;
using FillingStation.Core.Vehicles;
using FillingStation.DAL.Models;
using FillingStation.Extensions;

namespace FillingStation.Helpers
{
    public enum UpdateState
    {
        Placed,
        CanPlace,
        CannotPlace,
        Leaved
    }

    public sealed class PatternToImageBinding : INotifyPropertyChanged
    {
        private static readonly BitmapImage _cancelBitmapImage =
            new BitmapImage(new Uri("/Assets/cancel.png", UriKind.Relative));

        private PatternToImageBinding(IPattern pattern, Image image)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            if (image == null) throw new ArgumentNullException("image");

            Pattern = pattern;
            Image = image;
            Image.Stretch = Stretch.Fill;
            Image.Height *= pattern.Height;
            Image.Width *= pattern.Width;

            SetImage(pattern.ImagePath);
            Update();
            Pattern.Property.PropertyChanged += Property_PropertyChanged;
        }

        private void SetImage(string imageName)
        {
            PlacedImage = new BitmapImage(new Uri("/Assets/Patterns/" + imageName, UriKind.Relative));
            CanPlaceImage = PlacedImage;
            CannotPlaceImage = _cancelBitmapImage;
            LeavedImage = null;
        }

        public Image Image { get; protected set; }
        public IPattern Pattern { get; protected set; }

        public BitmapImage PlacedImage { get; protected set; }
        public BitmapImage CanPlaceImage { get; protected set; }
        public BitmapImage CannotPlaceImage { get; protected set; }
        public BitmapImage LeavedImage { get; protected set; }

        public int DX { get; set; }
        public int DY { get; set; }

        private UpdateState _currentState;

        public UpdateState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                UpdateImage(value);
                OnPropertyChanged();
            }
        }

        private void Update()
        {
            if (Pattern.Property is ITurnProperty)
            {
                SetRotation((Pattern.Property as ITurnProperty).Angle);
            }
            if (Pattern.Property is TankProperty)
            {
                SetFuelProperty((Pattern.Property as TankProperty).Fuel);
            }
            UpdateImage();
        }

        private void UpdateImage()
        {
            UpdateImage(CurrentState);
        }

        private void UpdateImage(UpdateState state)
        {
            BitmapImage selectedImageSource = null;
            Image.Opacity = 1;
            switch (state)
            {
                case UpdateState.CanPlace:
                    Image.Opacity = 0.5;
                    selectedImageSource = CanPlaceImage;
                    break;
                case UpdateState.CannotPlace:
                    selectedImageSource = CannotPlaceImage;
                    break;
                case UpdateState.Placed:
                    selectedImageSource = PlacedImage;
                    break;
                case UpdateState.Leaved:
                    selectedImageSource = LeavedImage;
                    break;
            }
            Image.Source = selectedImageSource;
        }

        public void CloneFrom(PatternToImageBinding anotherBinding)
        {
            Pattern.Property.Clone(anotherBinding.Pattern.Property);

            PlacedImage = anotherBinding.PlacedImage != null
                ? anotherBinding.PlacedImage.CloneCurrentValue()
                : null;
            CanPlaceImage = anotherBinding.CanPlaceImage != null
                ? anotherBinding.CanPlaceImage.CloneCurrentValue()
                : null;
            //CannotPlaceImage = anotherBinding.CannotPlaceImage != null ? 
            //    anotherBinding.CannotPlaceImage.CloneCurrentValue() :
            //    null;
            //LeavedImage = anotherBinding.LeavedImage != null ? 
            //    anotherBinding.LeavedImage.CloneCurrentValue() :
            //    null;

            CurrentState = anotherBinding.CurrentState;

            Update();
        }

        private void Property_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTurnPropertyIfNeeded(e.PropertyName);
            UpdateImagePathPropertyIfNeeded_HACK(e.PropertyName);
            UpdateImage();
        }

        #region Helper Methods

        private void UpdateTurnPropertyIfNeeded(string propertyName)
        {
            var properties = typeof (ITurnProperty).GetProperties();
            var property = properties.FirstOrDefault(prop => prop.PropertyType == typeof (Rotation));
            if (property != null && property.Name == propertyName)
            {
                var rotation = (Rotation) property.GetValue(Pattern.Property);
                SetRotation(rotation);
            }
        }

        //HACK
        private void UpdateImagePathPropertyIfNeeded_HACK(string propertyName)
        {
            var properties = typeof (TankProperty).GetProperties();
            var property = properties.FirstOrDefault(prop => prop.PropertyType == typeof (Fuel));
            if (property != null && property.Name == propertyName)
            {
                var fuel = (Fuel) property.GetValue(Pattern.Property);
                SetFuelProperty(fuel);
            }
        }

        private void SetFuelProperty(Fuel fuel)
        {
            var suffix = "";
            if (fuel == Fuel.A92)
            {
                suffix = "_a92.png";
            }
            if (fuel == Fuel.A95)
            {
                suffix = "_a95.png";
            }
            if (fuel == Fuel.A98)
            {
                suffix = "_a98.png";
            }
            if (fuel == Fuel.Diesel)
            {
                suffix = "_d.png";
            }

            SetImage(Pattern.ImagePath.Replace(".png", suffix));
        }

        private void SetRotation(Rotation rotation)
        {
            Image.Rotate(rotation);
        }

        #endregion

        #region Static methods

        public static PatternToImageBinding Load(IPattern pattern, Image image, PatternToImageBinding cloneFrom = null)
        {
            var result = new PatternToImageBinding(pattern, image);

            if (cloneFrom != null)
            {
                image.Margin = cloneFrom.Image.Margin;
                result.CloneFrom(cloneFrom);
            }
            return result;
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
