using System;
using System.Windows.Media.Imaging;
using FillingStation.Extensions;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Pathes
{
    public class InvertedSegmentPath : SegmentPath
    {
        public InvertedSegmentPath(Vector2 segmentCenter, float radius, int segmentCount = 1,
            Rotation turn = Rotation.Rotate0, Vector2 rotationPoint = new Vector2())
            : base(segmentCenter, radius, segmentCount, rotationPoint)
        {
            Turn = turn;
        }

        public Rotation Turn { get; private set; }

        protected override BasePath GetNormalInverted()
        {
            return new RotatedSegmentPath(SegmentCenter, Radius, SegmentCount, Turn, RotationPoint);
        }

        protected override Vector2 NormalEnter
        {
            get { return Transform(base.NormalEnter, SegmentCenter, SegmentCount, Turn); }
        }

        protected override Vector2 NormalWait
        {
            get { return Transform(base.NormalWait, SegmentCenter, SegmentCount, Turn); }
        }

        protected override Vector2 NormalExit
        {
            get { return Transform(base.NormalExit, SegmentCenter, SegmentCount, Turn); }
        }

        protected override Vector2 GetNormalPoint(Vector2 startPoint, float road)
        {
            startPoint = TransformBack(startPoint, SegmentCenter, SegmentCount, Turn);
            return Transform(base.GetNormalPoint(startPoint, road), SegmentCenter, SegmentCount, Turn); ;
        }

        protected override float GetNormalRoad(Vector2 fromPoint, Vector2 toPoint)
        {
            fromPoint = TransformBack(fromPoint, SegmentCenter, SegmentCount, Turn);
            toPoint = TransformBack(toPoint, SegmentCenter, SegmentCount, Turn);
            return base.GetNormalRoad(fromPoint, toPoint);
        }

        protected override float GetNormalTurn(Vector2 rotationPoint)
        {
            rotationPoint = TransformBack(rotationPoint, SegmentCenter, SegmentCount, Turn);
            return -base.GetNormalTurn(rotationPoint) + Turn.ToRadAngle() + Rotation.Rotate90.ToRadAngle();
        }

        private static Vector2 Transform(Vector2 vector2, Vector2 segmentCenter, int segmentCount, Rotation turn)
        {
            var res = Switch(vector2 - segmentCenter, segmentCount) + segmentCenter;
            return res.Turn(turn, segmentCenter);
        }

        private static Vector2 TransformBack(Vector2 vector2, Vector2 segmentCenter, int segmentCount, Rotation turn)
        {
            vector2 = vector2.TurnBack(turn, segmentCenter);
            return Switch(vector2 - segmentCenter, segmentCount) + segmentCenter;
        }

        private static Vector2 Switch(Vector2 vector2, int segmentCount)
        {
            if (!(1 <= segmentCount && segmentCount <= 4)) throw new ArgumentException("segmentCount must be from 1 to 4", "segmentCount");

            var res = Vector2.Zero;
            if (segmentCount == 1)
            {
                res = new Vector2(vector2.Y, vector2.X);
            }
            if (segmentCount == 2)
            {
                res = new Vector2(-vector2.X, vector2.Y);
            }
            if (segmentCount == 3)
            {
                res = new Vector2(-vector2.Y, -vector2.X);
            }
            if (segmentCount == 4)
            {
                res = new Vector2(vector2.X, -vector2.Y);
            }
            return res;
        }
    }
}