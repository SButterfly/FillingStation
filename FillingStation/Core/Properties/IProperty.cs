using System.ComponentModel;
using Newtonsoft.Json;

namespace FillingStation.Core.Properties
{
    public interface IProperty : INotifyPropertyChanged
    {
        [JsonIgnore]
        string PatternName { get; }

        void Clone(IProperty property);
    }
}