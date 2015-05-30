using System;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Pathes
{
    public class LinePath : BasePath
    {
        private readonly Vector2 _enter;
        private readonly Vector2 _wait;
        private readonly Vector2 _exit;

        private readonly float A;
        private readonly float B;
        private readonly float C;

        public LinePath(Vector2 startPosition, Vector2 endPosition) 
            : this(startPosition, endPosition, Vector2.Zero)
        { }

        public LinePath(Vector2 startPosition, Vector2 endPosition, Vector2 rotateCenter)
            : base(rotateCenter)
        {
            _enter = startPosition;
            _exit = endPosition;
            _wait = new Vector2((_exit.X + _enter.X) / 2f, (_exit.Y + _enter.Y) / 2f);

            A = startPosition.Y - endPosition.Y;
            B = endPosition.X - startPosition.X;
            C = -A*startPosition.X - B*startPosition.Y;

            float z = (float) Math.Sqrt(A * A + B * B);
            A /= z;
            B /= z;
            C /= z;
        }

        protected override Vector2 NormalEnter { get { return _enter; } }
        protected override Vector2 NormalWait { get { return _wait; } }
        protected override Vector2 NormalExit { get { return _exit; } }

        protected override Vector2 GetNormalPoint(Vector2 startPoint, float road)
        {
            if (!Contains(startPoint)) throw new ArgumentException(Strings.Exception_point_is_not_on_path, "startPoint");

            var normVector = new Vector2(-B * road, A * road);

            return startPoint - normVector;
        }

        protected override float GetNormalRoad(Vector2 fromPoint, Vector2 toPoint)
        {
            if (!Contains(fromPoint)) throw new ArgumentException(Strings.Exception_point_is_not_on_path, "fromPoint");
            if (!Contains(toPoint)) throw new ArgumentException(Strings.Exception_point_is_not_on_path, "toPoint");

            double x1 = fromPoint.X;
            double y1 = fromPoint.Y;

            double x2 = toPoint.X;
            double y2 = toPoint.Y;

            int sign = GetSign(fromPoint, toPoint);

            return sign*(float)Math.Sqrt((x1 - x2)*(x1 - x2) + (y1 - y2)*(y1 - y2));
        }

        private int GetSign(Vector2 fromPoint, Vector2 toPoint)
        {
            var baseVector = NormalExit - NormalEnter;
            var findVector = toPoint - fromPoint;

            double bx = baseVector.X;
            double by = baseVector.Y;

            double fx = findVector.X;
            double fy = findVector.Y;

            bool isOnOneLine = Math.Abs(fx*by - fy*bx) < 1e-5;

            if (!isOnOneLine)
                return 0;

            bool isFxBetween = bx > 0 ? 0 <= fx && fx <= bx : bx <= fx && fx <= 0;
            bool isFyBetween = by > 0 ? 0 <= fy && fy <= by : by <= fy && fy <= 0;

            return isFxBetween && isFyBetween ? 1 : -1;
        }

        protected override float GetNormalTurn(Vector2 rotationPoint)
        {
            if (!Contains(rotationPoint)) throw new ArgumentException(Strings.Exception_point_is_not_on_path, "rotationPoint");

            var newPoint = rotationPoint - _enter;

            var angle = -(float)Math.Atan(newPoint.Y / newPoint.X);
            return angle;
        }

        private bool Contains(Vector2 point)
        {
            const double EPS = 1e-4;

            return Math.Abs(A*point.X + B*point.Y + C) < EPS;
        }

        protected override BasePath GetNormalInverted()
        {
            return new LinePath(NormalExit, NormalEnter, RotationPoint);
        }
    }
}