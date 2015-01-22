using System.ComponentModel;
using System.Runtime.CompilerServices;
using FillingStation.Annotations;

namespace FillingStation.Core.Properties
{
    public abstract class BaseProperty : IProperty
    {
        public abstract string PatternName { get; }

        public virtual void Clone(IProperty property)
        {

        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}