using FillingStation.Localization;

namespace FillingStation.Core.Properties
{
    public sealed class ColumnProperty : BaseTurnProperty
    {
        public override string PatternName
        {
            get { return Strings.Column; }
        }
    }
}