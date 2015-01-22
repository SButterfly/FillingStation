using System.Windows.Media.Imaging;

namespace FillingStation.Core.Properties
{
    public interface ITurnProperty : IProperty
    {
        Rotation Angle { get; set; }
    }
}