using System.Windows.Media.Imaging;
using FillingStation.Core.Pathes;
using FillingStation.Core.Properties;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    public sealed class RoadTPattern : BaseGameRoadPattern<RoadTProperty>
    {
        public RoadTPattern()
            : base(1, 1, Strings.RoadT_path)
        {
            var firstLine = new LinePath(new Vector2(0.5f, 1f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f));
            var secondLine = firstLine.Inverted();

            var rotateLine1 = new RotatedSegmentPath(new Vector2(0f, 1f), 0.5f, 1, Rotation.Rotate270, new Vector2(0.5f, 0.5f));
            var rotateLine1Inverted = rotateLine1.Inverted();
            var rotateLine2 = new RotatedSegmentPath(new Vector2(0f, 0f), 0.5f, 1, Rotation.Rotate0, new Vector2(0.5f, 0.5f));
            var rotateLine2Inverted = rotateLine2.Inverted();

            firstLine.BindToTurnProperty(Property);
            secondLine.BindToTurnProperty(Property);

            rotateLine1.BindToTurnProperty(Property);
            rotateLine1Inverted.BindToTurnProperty(Property);

            rotateLine2.BindToTurnProperty(Property);
            rotateLine2Inverted.BindToTurnProperty(Property);

            Paths = new BasePath[] {
                firstLine,
                secondLine,
                rotateLine1,
                rotateLine1Inverted,
                rotateLine2,
                rotateLine2Inverted
            };
        }
    }
}