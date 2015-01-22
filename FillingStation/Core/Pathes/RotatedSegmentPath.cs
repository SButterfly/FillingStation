using System.Windows.Media.Imaging;
using FillingStation.Extensions;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Pathes
{
    public class RotatedSegmentPath : SegmentPath
    {
        public RotatedSegmentPath(Vector2 segmentCenter, float radius, int segmentCount = 1, Rotation turn = Rotation.Rotate0, Vector2 rotationPoint = new Vector2())
            : base(segmentCenter, radius, segmentCount, rotationPoint)
        {
            Turn = turn;
        }

        public Rotation Turn { get; private set; }

        protected override BasePath GetNormalInverted()
        {
            return new InvertedSegmentPath(SegmentCenter, Radius, SegmentCount, Turn, RotationPoint);
        }

        protected override Vector2 NormalEnter
        {
            get { return base.NormalEnter.Turn(Turn, SegmentCenter); }
        }

        protected override Vector2 NormalWait
        {
            get { return base.NormalWait.Turn(Turn, SegmentCenter); }
        }

        protected override Vector2 NormalExit
        {
            get { return base.NormalExit.Turn(Turn, SegmentCenter); }
        }

        protected override Vector2 GetNormalPoint(Vector2 startPoint, float road)
        {
            startPoint = startPoint.TurnBack(Turn, SegmentCenter);
            return base.GetNormalPoint(startPoint, road).Turn(Turn, SegmentCenter);
        }

        protected override float GetNormalRoad(Vector2 fromPoint, Vector2 toPoint)
        {
            fromPoint = fromPoint.TurnBack(Turn, SegmentCenter);
            toPoint = toPoint.TurnBack(Turn, SegmentCenter);
            return base.GetNormalRoad(fromPoint, toPoint);
        }

        protected override float GetNormalTurn(Vector2 rotationPoint)
        {
            rotationPoint = rotationPoint.TurnBack(Turn, SegmentCenter);
            return base.GetNormalTurn(rotationPoint) + Turn.ToRadAngle();
        }
    }
}