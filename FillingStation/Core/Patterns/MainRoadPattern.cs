using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FillingStation.Core.Pathes;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    public sealed class MainRoadPattern : BasePattern, IGameRoadPattern
    {
        public MainRoadPattern()
            : base(1, 1)
        {
            var secondLine = new LinePath(new Vector2(1f, 0.5f), new Vector2(0f, 0.5f));

            var rotateLine = new RotatedSegmentPath(new Vector2(1f, 0f), 0.5f, 1, Rotation.Rotate90);
            var rotateLine2 = new RotatedSegmentPath(new Vector2(0f, 0f), 0.5f, 1, Rotation.Rotate0);

            Paths = new BasePath[] {
                rotateLine,
                rotateLine2,
                secondLine
            };
        }

        public IList<BasePath> Paths { get; private set; }
    }
}