using FillingStation.Core.Pathes;
using FillingStation.Core.Properties;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    public sealed class RoadOutPattern : BaseGameRoadPattern<RoadOutProperty>
    {
        public RoadOutPattern()
            : base(2, 1, Strings.RoadOut_path)
        {
            var firstLine = new LinePath(new Vector2(0.5f, 0f), new Vector2(0.5f, 1f));

            Paths = new BasePath[] {
                firstLine
            };
        }
    }
}