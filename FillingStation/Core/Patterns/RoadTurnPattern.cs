using System.Windows.Media.Imaging;
using FillingStation.Core.Pathes;
using FillingStation.Core.Properties;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    public sealed class RoadTurnPattern : BaseGameRoadPattern<RoadTurnProperty>
    {
        public RoadTurnPattern()
            : base(1, 1, Strings.RoadTurn_path)
        {
            var rotateLine = new RotatedSegmentPath(new Vector2(0f, 1f), 0.5f, 1, Rotation.Rotate270, new Vector2(0.5f, 0.5f));
            var rotateLineInverted = rotateLine.Inverted();

            rotateLine.BindToTurnProperty(Property);
            rotateLineInverted.BindToTurnProperty(Property);

            Paths = new BasePath[] {
                rotateLine,
                rotateLineInverted
            };
        }
    }
}