using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FillingStation.Core.Models;
using FillingStation.Core.Patterns;
using FillingStation.Core.Properties;
using FillingStation.Extensions;
using FillingStation.Helpers;
using FillingStation.Localization;
using FillingStation.Properties;
using Point = System.Drawing.Point;

namespace FillingStation.Controls
{
    public partial class ConstructionControl : UserControl
    {
        private const string LTAG = "ConstructionControl";

        private static readonly SolidColorBrush _dottedLineBrush = new SolidColorBrush(Colors.CadetBlue);

        private readonly float _cellWidth = 40f;
        private readonly float _cellHeight = 40f;

        public ConstructionControl()
        {
            InitializeComponent();
            field.Children.Add(_selectionBorder);

            _cellWidth = Settings.Default.cellWidth;
            _cellHeight = Settings.Default.cellWidth;

            IsEnabledChanged += (sender, args) =>
            {
                disanabledGrid.Visibility = !IsEnabled ? Visibility.Visible : Visibility.Collapsed;
                disanabledGrid2.Visibility = !IsEnabled ? Visibility.Visible : Visibility.Collapsed;
            };

            FSModel = null;
        }

        #region Properties

        public static readonly DependencyProperty ModelProperty =
                          DependencyProperty.Register("FSModel", typeof(FSModel), typeof(ConstructionControl), new PropertyMetadata(null));

        public FSModel FSModel
        {
            get { return (FSModel)GetValue(ModelProperty); }
            set
            {
                if (value != null)
                {
                    DrawModel(value);
                    borderField.Visibility = Visibility.Visible;
                }
                else
                {
                    DrawModel(new FSModel(1, 1));
                    borderField.Visibility = Visibility.Collapsed;
                }

                SelectedBinding = null;
                SetValue(ModelProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedPatternProperty =
                          DependencyProperty.Register("SelectedPattern", typeof(IPattern), typeof(ConstructionControl), new PropertyMetadata(null));

        public IPattern SelectedPattern
        {
            get { return (IPattern)GetValue(SelectedPatternProperty); }
            private set
            {
                SetValue(SelectedPatternProperty, value);
                propertyControl.Property = value != null ? value.Property : null;
            }
        }

        #endregion

        #region Methods

        public Stream GetImageStream()
        {
            RemoveDottedLines();
            field.Children.Remove(_selectionBorder);
            field.UpdateLayout();

            Viewbox viewbox = new Viewbox();
            borderField.Child = null;
            viewbox.Child = field;
            viewbox.Measure(new Size(field.Width, field.Height));
            viewbox.Arrange(new Rect(0, 0, field.Width, field.Height));
            viewbox.UpdateLayout();

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)field.Width, (int)field.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(viewbox);

            viewbox.Child = null;
            borderField.Child = field;
            CreateDottedLines(FSModel.Height, FSModel.Width);
            field.Children.Add(_selectionBorder);

            var memoryStream = new MemoryStream();
            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            png.Save(memoryStream);
            memoryStream.Flush();
            memoryStream.Position = 0;

            return memoryStream;
        }

        #endregion

        #region Private methods

        private void CreateField(int modelHeight, int modelWidth)
        {
            CleanField();

            field.Height = modelHeight * _cellHeight;
            field.Width = modelWidth * _cellWidth;
        }

        private Line[] dottedLinesHorizontal = new Line[0];
        private Line[] dottedLinesVerticals = new Line[0];

        private void CreateDottedLines(int modelHeight, int modelWidth)
        {
            dottedLinesHorizontal = new Line[modelHeight - 1];
            dottedLinesVerticals = new Line[modelWidth - 1];

            for (int i = 1; i < modelHeight; i++)
            {
                var line = CreateDottedLine();
                line.X2 = field.Width;
                line.Y1 = i * _cellHeight;
                line.Y2 = i * _cellHeight;

                dottedLinesHorizontal[i - 1] = line;
                field.Children.Insert(0, line);
            }

            for (int i = 1; i < modelWidth; i++)
            {
                var line = CreateDottedLine();
                line.Y2 = field.Height;
                line.X1 = i * _cellWidth;
                line.X2 = i * _cellWidth;

                dottedLinesVerticals[i - 1] = line;
                field.Children.Insert(0, line);
            }
        }

        private void RemoveDottedLines()
        {
            foreach (var line in dottedLinesHorizontal)
            {
                field.Children.Remove(line);
            }
            foreach (var line in dottedLinesVerticals)
            {
                field.Children.Remove(line);
            }
            dottedLinesHorizontal = null;
            dottedLinesVerticals = null;
        }

        private void CreateMainRoad(int modelWidth)
        {
            var grid = new Grid();
            grid.Width = _cellWidth * modelWidth;
            grid.Height = 2*_cellHeight;
            grid.Margin = new Thickness(0, field.Height, 0, 0);
            
            for (int i = 0; i < modelWidth; i++)
            {
                var image = new Image
                {
                    Width = _cellWidth,
                    Height = 2*_cellHeight,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Source = new BitmapImage(new Uri("/Assets/main_road.png", UriKind.Relative)),
                    Margin = new Thickness(i * _cellWidth, 0, 0, 0)
                };
                image.UpdateLayout();
                grid.Children.Add(image);
            }

            field.Height += grid.Height;
            field.Children.Add(grid);
        }

        private void CleanField()
        {
            field.Children.Clear();
        }

        private void DrawModel(FSModel model)
        {
            CreateField(model.Height, model.Width);
            CreateDottedLines(model.Height, model.Width);
            CreateMainRoad(model.Width);
            PlacePatterns(model);
        }

        #endregion

        #region Draw methods

        private Line CreateDottedLine()
        {
            Line line = new Line()
            {
                Stroke = _dottedLineBrush,
                StrokeDashArray = DoubleCollection.Parse("2 4"),
                SnapsToDevicePixels = true
            };

            return line;
        }

        #endregion
        
        #region Place Methods

        private void ConstructionControl_OnKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                if (Mode == ConstructionMode.None && SelectedBinding != null)
                {
                    SelectedBinding = null;
                }
                if (Mode == ConstructionMode.MultiPlacing)
                {
                    if (SelectedMenuButton != null)
                    {
                        SelectedMenuButton.IsChecked = false;
                        SelectedMenuButton = null;
                    }
                    Mode = ConstructionMode.None;
                }
            }
        }

        enum ConstructionMode
        {
            None,
            Placing,
            MultiPlacing
        }

        private ConstructionMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                string additionText = null;
                if (_mode == ConstructionMode.MultiPlacing)
                {
                    additionText = Strings.Construction_placeOnCell;
                }
                if (_mode == ConstructionMode.Placing)
                {
                    additionText = Strings.Construction_placeOnNewCell;
                }
                additionTextBlock.Text = additionText;
            }
        }

        private ToggleButton SelectedMenuButton { get; set; }

        private readonly Border _selectionBorder = new Border
        {
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(2),
            Visibility = Visibility.Collapsed,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };

        private PatternToImageBinding _selectedBinding;
        private PatternToImageBinding SelectedBinding
        {
            get { return _selectedBinding; }
            set
            {
                if (_selectedBinding != null)
                {
                    _selectedBinding.PropertyChanged -= SelectedBindingOnPropertyChanged;
                    _selectionBorder.Visibility = Visibility.Collapsed;
                }

                _selectedBinding = value;
                SelectedPattern = value != null ? value.Pattern : null;
                if (_selectedBinding != null)
                {
                    _selectedBinding.PropertyChanged += SelectedBindingOnPropertyChanged;
                    SelectedBindingOnPropertyChanged(this, null);

                    var top = _selectedBinding.Image.Margin.Top;
                    var left = _selectedBinding.Image.Margin.Left;
                    _selectionBorder.Margin = new Thickness(left - _selectionBorder.BorderThickness.Left, top - _selectionBorder.BorderThickness.Top, 0, 0);

                    _selectionBorder.Height = _selectedBinding.Image.Height + _selectionBorder.BorderThickness.Top + _selectionBorder.BorderThickness.Bottom;
                    _selectionBorder.Width = _selectedBinding.Image.Width + _selectionBorder.BorderThickness.Left + _selectionBorder.BorderThickness.Right;

                    SelectedBindingToTop(SelectedBinding);
                }
                else
                {
                    _selectionBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SelectedBindingOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_selectedBinding.CurrentState == UpdateState.Leaved || _selectedBinding.CurrentState == UpdateState.CannotPlace)
            {
                _selectionBorder.Visibility = Visibility.Collapsed;
            }
            if (_selectedBinding.CurrentState == UpdateState.Placed)
            {
                _selectionBorder.Visibility = Visibility.Visible;
                _selectionBorder.Opacity = 1d;
            }
            if (_selectedBinding.CurrentState == UpdateState.CanPlace)
            {
                _selectionBorder.Visibility = Visibility.Visible;
                _selectionBorder.Opacity = 0.5d;
            }
        }

        private void MenuItem_OnChecked(object sender, RoutedEventArgs e)
        {
            if (SelectedMenuButton != null)
            {
                SelectedMenuButton.IsChecked = false;
            }

            if (Mode == ConstructionMode.MultiPlacing || Mode == ConstructionMode.None)
            {
                Logger.WriteLine(LTAG, "OnChecked");

                SelectedMenuButton = (ToggleButton)sender;

                var image = CreateImage();
                var pattern = CreatePattern(SelectedMenuButton.Tag);
                SelectedBinding = PatternToImageBinding.Load(pattern, image);
                SelectedBinding.CurrentState = UpdateState.Leaved;

                Mode = ConstructionMode.MultiPlacing;
            }
        }

        private void MenuItem_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (Mode == ConstructionMode.MultiPlacing)
            {
                Logger.WriteLine(LTAG, "OnUnchecked");

                field.Children.Remove(SelectedBinding.Image);
                
                SelectedMenuButton = null;
                SelectedBinding = null;

                Mode = ConstructionMode.None;
            }
        }

        private void field_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(sender as UIElement);
            var point = GetPosition(position, _cellWidth, _cellHeight);

            if (Mode == ConstructionMode.Placing)
            {
                Logger.WriteLine(LTAG, "MouseUp Placing " + point);

                point.X = Math.Min(point.X - SelectedBinding.DX, FSModel.Width - SelectedBinding.Pattern.Width);
                point.Y = Math.Min(point.Y - SelectedBinding.DY, FSModel.Height - SelectedBinding.Pattern.Height);

                if (FSModel.CanPlace(SelectedBinding.Pattern, point) || FSModel.Get(point) == SelectedBinding.Pattern)
                {
                    if (!FSModel.Contains(SelectedBinding.Pattern))
                        FSModel.Add(SelectedBinding.Pattern, point);

                    Place(point, SelectedBinding);
                    SelectedBinding.CurrentState = UpdateState.Placed;
                }
                else
                {
                    field.Children.Remove(SelectedBinding.Image);
                    SelectedBinding = null;
                }

                Mode = ConstructionMode.None;
            }
            if (Mode == ConstructionMode.MultiPlacing)
            {
                Logger.WriteLine(LTAG, "MouseUp MultiPlacing " + point);

                point.X = Math.Min(point.X - SelectedBinding.DX, FSModel.Width - SelectedBinding.Pattern.Width);
                point.Y = Math.Min(point.Y - SelectedBinding.DY, FSModel.Height - SelectedBinding.Pattern.Height);

                if (FSModel.CanPlace(SelectedBinding.Pattern, point))
                {
                    FSModel.Add(SelectedBinding.Pattern, point);
                    Place(point, SelectedBinding);
                    SelectedBinding.CurrentState = UpdateState.Placed;

                    var image = CreateImage();
                    var pattern = CreatePattern(SelectedMenuButton.Tag);
                    SelectedBinding = PatternToImageBinding.Load(pattern, image, SelectedBinding);
                    SelectedBinding.CurrentState = UpdateState.CanPlace;
                }
            }
        }

        private void field_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(sender as UIElement);
            var point = GetPosition(position, _cellWidth, _cellHeight);

            if (Mode == ConstructionMode.None)
            {
                Logger.WriteLine(LTAG, "MouseDown None " + point);

                var binding = Remove(point);
                if (binding != null)
                {
                    SelectedBinding = binding;

                    var patternPoint = FSModel.Get(binding.Pattern);
                    SelectedBinding.DX = point.X - patternPoint.X;
                    SelectedBinding.DY = point.Y - patternPoint.Y;

                    Mode = ConstructionMode.Placing;
                    field_OnMouseMove(sender, e);
                }
            }
        }

        private void field_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(sender as UIElement);
            var point = GetPosition(position, _cellWidth, _cellHeight);

            if (Mode == ConstructionMode.None)
            {
                Logger.WriteLine(LTAG, "MouseRightDown None " + point);

                SelectedBinding = Remove(point);
                if (SelectedBinding != null)
                {
                    FSModel.Remove(SelectedBinding.Pattern);
                    SelectedBinding.CurrentState = UpdateState.Leaved;
                    SelectedBinding = null;
                }
            }
            if (Mode == ConstructionMode.MultiPlacing)
            {
                SelectedBinding.CurrentState = UpdateState.Leaved;
                SelectedBinding = null;
                Mode = ConstructionMode.None; 

                if (SelectedMenuButton != null)
                {
                    SelectedMenuButton.IsChecked = false;
                    SelectedMenuButton = null;
                }
            }
        }

        private void field_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Mode == ConstructionMode.MultiPlacing)
            {
                Logger.WriteLine(LTAG, "MouseLeave");

                SelectedBinding.CurrentState = UpdateState.Leaved;
            }

            if (Mode == ConstructionMode.Placing)
            {
                Logger.WriteLine(LTAG, "MouseLeave");

                SelectedBinding.CurrentState = UpdateState.Leaved;
                SelectedBinding = null;
                Mode = ConstructionMode.None;
            }
        }

        private void field_OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(sender as UIElement);
            var point = GetPosition(position, _cellWidth, _cellHeight);

            if (Mode == ConstructionMode.Placing || Mode == ConstructionMode.MultiPlacing)
            {
                point.X = Math.Min(point.X - SelectedBinding.DX, FSModel.Width - SelectedBinding.Pattern.Width);
                point.Y = Math.Min(point.Y - SelectedBinding.DY, FSModel.Height - SelectedBinding.Pattern.Height);

                var margin = SelectedBinding.Image.Margin;

                if (margin.Left != point.X * _cellWidth || margin.Top != point.Y * _cellHeight)
                {
                    if (FSModel.Contains(SelectedBinding.Pattern))
                    {
                        Logger.WriteLine(LTAG, "FSModel removed {0} from point {1}", SelectedBinding.Pattern, point);
                        FSModel.Remove(SelectedBinding.Pattern);
                    }

                    Logger.WriteLine(LTAG, "MouseMove " + point);

                    SelectedBinding.Image.Margin = new Thickness(point.X * _cellWidth, point.Y * _cellHeight, 0, 0);

                    var top = SelectedBinding.Image.Margin.Top;
                    var left = SelectedBinding.Image.Margin.Left;
                    _selectionBorder.Margin = new Thickness(left - _selectionBorder.BorderThickness.Left, top - _selectionBorder.BorderThickness.Top, 0, 0);
                }

                SelectedBinding.CurrentState = FSModel.CanPlace(SelectedBinding.Pattern, point) || FSModel.Get(point) == SelectedBinding.Pattern
                    ? UpdateState.CanPlace
                    : UpdateState.CannotPlace;
            }
        }

        private void field_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (SelectedBinding != null)
            {
                Logger.WriteLine(LTAG, "MouseTurn");
                var turnProperty = SelectedBinding.Pattern.Property as ITurnProperty;
                if (turnProperty != null)
                {
                    var sumAngle = e.Delta > 0
                        ? Rotation.Rotate90
                        : Rotation.Rotate270;

                    turnProperty.Angle = turnProperty.Angle.Sum(sumAngle);
                }
            }
        }

        private PatternToImageBinding[,] _bindings;
        private ConstructionMode _mode;

        private void PlacePatterns(FSModel model)
        {
            _bindings = new PatternToImageBinding[model.Width, model.Height];

            foreach (var pattern in model.Patterns)
            {
                var image = CreateImage();
                var binding = PatternToImageBinding.Load(pattern, image);

                Point point = model.Get(pattern);
                Place(point, binding);
                binding.CurrentState = UpdateState.Placed;
                binding.Image.Margin = new Thickness(point.X * _cellWidth, point.Y * _cellHeight, 0, 0);
            }
        }

        private PatternToImageBinding Remove(Point point)
        {
            if (0 <= point.X && point.X < FSModel.Width && 0 <= point.Y && point.Y < FSModel.Height)
            {
                var binding = _bindings[point.X, point.Y];
                if (binding != null)
                {
                    for (int i = 0; i < binding.Pattern.Width && point.X + i < FSModel.Width; i++)
                    {
                        for (int j = 0; j < binding.Pattern.Height && point.Y + j < FSModel.Height; j++)
                        {
                            if (_bindings[point.X + i, point.Y + j] == binding)
                            {
                                _bindings[point.X + i, point.Y + j] = null;
                            }
                        }
                    }
                    for (int i = 0; i < binding.Pattern.Width && point.X - i >= 0; i++)
                    {
                        for (int j = 0; j < binding.Pattern.Height && point.Y - j >= 0; j++)
                        {
                            if (_bindings[point.X - i, point.Y - j] == binding)
                            {
                                _bindings[point.X - i, point.Y - j] = null;
                            }
                        }
                    }
                    return binding;
                }
            }

            return null;
        }

        private void Place(Point point, PatternToImageBinding binding)
        {
            for (int i = 0; i < binding.Pattern.Width; i++)
            {
                for (int j = 0; j < binding.Pattern.Height; j++)
                {
                    _bindings[point.X + i, point.Y + j] = binding;
                }
            }
        }

        #endregion

        #region Helper methods

        private IPattern CreatePattern(object obj)
        {
            var bindingType = Type.GetType(typeof(IPattern).Namespace + "." + obj.ToString() + "Pattern");
            var result = Activator.CreateInstance(bindingType) as IPattern;

            if (result == null) throw new ArgumentException();

            return result;
        }

        private Image CreateImage()
        {
            var image = new Image
            {
                Width = _cellWidth,
                Height = _cellHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            image.UpdateLayout();
            field.Children.Add(image);
            return image;
        }

        private void SelectedBindingToTop(PatternToImageBinding selectedBinding)
        {
            field.Children.Remove(selectedBinding.Image);
            field.Children.Remove(_selectionBorder);
            field.Children.Add(selectedBinding.Image);
            field.Children.Add(_selectionBorder);
        }

        private Point GetPosition(System.Windows.Point point, double cellWidth, double cellHeight)
        {
            int x = (int) (point.X / cellWidth);
            int y = (int) (point.Y / cellHeight);

            x = Math.Max(0, Math.Min(FSModel.Width, x));
            y = Math.Max(0, Math.Min(FSModel.Height, y));

            return new Point(x,y);
        }

        #endregion
    }
}
