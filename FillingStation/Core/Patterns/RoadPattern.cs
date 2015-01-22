using FillingStation.Core.Pathes;
using FillingStation.Core.Properties;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    public sealed class RoadPattern : BaseGameRoadPattern<RoadProperty>
    {
        public RoadPattern()
            : base(1, 1, Strings.Road_path)
        {
            var firstLine = new LinePath(new Vector2(0.5f, 1f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f));
            var secondLine = firstLine.Inverted();

            firstLine.BindToTurnProperty(Property);
            secondLine.BindToTurnProperty(Property);

            Paths = new BasePath[] {
                firstLine,
                secondLine
            };
        }
    }
}