using FillingStation.Core.Pathes;
using FillingStation.Core.Properties;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    public sealed class RoadInPattern : BaseGameRoadPattern<RoadInProperty>
    {
        public RoadInPattern()
            : base(2, 1, Strings.RoadIn_path)
        {
            var firstLine = new LinePath(new Vector2(0.5f, 1f), new Vector2(0.5f, 0f));

            Paths = new BasePath[] {
                firstLine
            };
        }
    }
}