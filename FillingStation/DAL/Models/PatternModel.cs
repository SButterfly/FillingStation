using FillingStation.Core.Patterns;

namespace FillingStation.DAL.Models
{
    public class PatternModel
    {
        public PatternModel(int x, int y, IPattern pattern)
        {
            X = x;
            Y = y;
            Pattern = pattern;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public IPattern Pattern { get; set; }
    }
}