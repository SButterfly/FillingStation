using System.ComponentModel;

namespace FillingStation.Core.Properties
{
    public interface IProperty : INotifyPropertyChanged
    {
        string PatternName { get; }

        void Clone(IProperty property);
    }
}