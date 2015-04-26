using FillingStation.Localization;
using Newtonsoft.Json;

namespace FillingStation.Core.Properties
{
    public class CashBoxProperty : BaseProperty
    {
        public override string PatternName
        {
            get { return Strings.CashBox; }
        }

        private int _cashBoxLimit = 500000;
        [JsonProperty("cash")]
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

        [JsonIgnore]
        public string CashBoxLimitName
        {
            get { return Strings.CashBox_limit; }
        }

        public override void Clone(IProperty property)
        {
            base.Clone(property);
            CashBoxLimit = (property as CashBoxProperty).CashBoxLimit;
        }
    }
}
