using FillingStation.Core.Properties;
using FillingStation.Localization;

namespace FillingStation.Core.Patterns
{
     public sealed class CashBoxPattern : BasePattern<CashBoxProperty>
    {
         public CashBoxPattern()
             : base(2, 2, Strings.CashBox_path)
         {
         }
    }
}
