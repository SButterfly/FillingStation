using System.Windows.Forms;

namespace FillingStation.Helpers
{
    public class OptimizedPanel : Panel
    {
        public OptimizedPanel()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Opaque, true);
        }
    }
}