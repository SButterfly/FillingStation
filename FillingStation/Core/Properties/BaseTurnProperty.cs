using System.Windows.Media.Imaging;

namespace FillingStation.Core.Properties
{
    public abstract class BaseTurnProperty : BaseProperty, ITurnProperty
    {
        private Rotation _angle;
        public Rotation Angle
        {
            get { return _angle; }
            set
            {
                if (value == _angle) return;
                _angle = value;
                OnPropertyChanged();
            }
        }

        public override void Clone(IProperty property)
        {
            if (property is ITurnProperty)
            {
                Angle = ((ITurnProperty) property).Angle;
            }
        }
    }
}