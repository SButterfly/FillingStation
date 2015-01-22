using FillingStation.Localization;

namespace FillingStation.Core.Properties
{
    public class CashBoxProperty : BaseProperty
    {
        public override string PatternName
        {
            get { return Strings.CashBox; }
        }

        private int _cashBoxLimit = 500000;
        public int CashBoxLimit
        {
            get { return _cashBoxLimit; }
            set
            {
                if (value == _cashBoxLimit) return;
                _cashBoxLimit = value;
                OnPropertyChanged();
            }
        }

        public string CashBoxLimitName
        {
            get { return Strings.CashBox_limit; }
        }
    }
}
