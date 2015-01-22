using FillingStation.Core.Pathes;
using FillingStation.Core.Properties;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    public sealed class TankPattern : BaseGameRoadPattern<TankProperty>
    {
        public TankPattern()
            : base(2, 1, Strings.Tank_path)
        {
            var firstLine = new LinePath(new Vector2(0.5f, 1f), new Vector2(0.5f, 0f));
            var secondLine = firstLine.Inverted();

            Paths = new BasePath[] {
                firstLine,
                secondLine
            };
        }
    }
}