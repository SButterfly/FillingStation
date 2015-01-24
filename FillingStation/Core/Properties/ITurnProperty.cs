using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace FillingStation.Core.Properties
{
    public interface ITurnProperty : IProperty
    {
        [JsonProperty("angle")]
        Rotation Angle { get; set; }
    }
}