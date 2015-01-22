using FillingStation.Core.Properties;
using FillingStation.Localization;

namespace FillingStation.Core.Patterns
{
    public class InfoTablePattern : BasePattern<InfoTableProperty>
    {
        public InfoTablePattern()
            : base(2, 2, Strings.InfoTable_path)
        {
        }
    }
}