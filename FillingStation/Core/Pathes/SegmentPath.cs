using System;
using System.Windows.Media.Imaging;
using FillingStation.Extensions;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Pathes
{
    public class SegmentPath : BasePath
    {
        private readonly double _startAngle;
        private readonly double _endAngle;
        
        public SegmentPath(Vector2 segmentCenter, float radius, int segmentCount = 1, Vector2 rotationPoint = new Vector2())
            : base(rotationPoint)
        {
            if (!(1 <= segmentCount && segmentCount <= 4)) throw new ArgumentOutOfRangeException("segmentCount", Strings.Exception_segment_count);

            _startAngle = 0;
            _endAngle = Math.PI * 90 * segmentCount / 180;

            Radius = radius;
            SegmentCount = segmentCount;
            SegmentCenter = segmentCenter;
        }

        public Vector2 SegmentCenter { get; private set; }
        public float Radius { get; private set; }
        public int SegmentCount { get; private set; }

        protected override BasePath GetNormalInverted()
        {
            return new InvertedSegmentPath(SegmentCenter, Radius, SegmentCount, Rotation.Rotate0, RotationPoint);
        }

        #region Overriding methods

        protected override Vector2 NormalEnter
        {
            get { return GetPoint(_startAngle); }
        }

        protected override Vector2 NormalWait
        {
            get
            {
                double angle = ((_startAngle + _endAngle)/2)%(2*Math.PI);
                return GetPoint(angle);
            }
        }

        protected override Vector2 NormalExit
        {
            get { return GetPoint(_endAngle); }
        }

        protected override Vector2 GetNormalPoint(Vector2 startPoint, float road)
        {
            double angle = road/Radius + GetAngle(startPoint);
            var result = GetPoint(angle);
            return result;
        }

        protected override float GetNormalRoad(Vector2 fromPoint, Vector2 toPoint)
        {
            double angle = GetAngle(toPoint) - GetAngle(fromPoint);

            double road = Radius * angle;
            return (float) road;
        }

        protected override float GetNormalTurn(Vector2 point)
        {
            float x = point.X - SegmentCenter.X;
            float y = point.Y - SegmentCenter.Y;

            double result = Math.Atan(x / y);
            return (float)-result;
        }

        #endregion

        #region Help methods

        public bool Contains(Vector2 vector2)
        {
            const double eps = 1e-4;

            float x = vector2.X - SegmentCenter.X;
            float y = vector2.Y - SegmentCenter.Y;

            bool isOnCircular = x * x + y * y - Radius * Radius <= eps;

            double angle = GetAngle(vector2);
            bool isInSegment = _startAngle - eps <= angle && angle <= _endAngle + eps;
            return isOnCircular && isInSegment;
        }

        private double GetAngle(Vector2 point)
        {
            float x = point.X - SegmentCenter.X;
            float y = point.Y - SegmentCenter.Y;

            double result = Math.Atan(x/y);
            return Rotation.Rotate90.ToRadAngle() - result;
        }

        private Vector2 GetPoint(double angle)
        {
            angle %= 2*Math.PI;

            float x = (float)(Radius * Math.Cos(angle));
            float y = (float)(Radius * Math.Sin(angle));

            return SegmentCenter + new Vector2(x, y);
        }

        #endregion
    }
}