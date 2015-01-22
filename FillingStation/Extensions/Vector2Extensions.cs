using System;
using Microsoft.Xna.Framework;
using Point = System.Drawing.Point;

namespace FillingStation.Extensions
{
    public static class Vector2Extensions
    {
        private const double _eps = 1e-4;

        public static bool IsNearBy(this Vector2 vector2, Vector2 anotherVector2, double eps = _eps)
        {
            float x1 = vector2.X;
            float y1 = vector2.Y;

            float x2 = anotherVector2.X;
            float y2 = anotherVector2.Y;

            var cc = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            return cc < eps;
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}