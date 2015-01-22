using System;
using System.Windows.Media.Imaging;
using FillingStation.Localization;
using Microsoft.Xna.Framework;

namespace FillingStation.Extensions
{
    public static class RotationExtensions
    {
        public static Rotation Sum(this Rotation rotation, Rotation sumRotation)
        {
            return (Rotation)(((int)rotation + (int)sumRotation)%4);
        }

        public static double ToAngle(this Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.Rotate0:
                    return 0;
                case Rotation.Rotate180:
                    return 180;
                case Rotation.Rotate270:
                    return 270;
                case Rotation.Rotate90:
                    return 90;
            }
            return 0;
        }

        public static Rotation ToRotation(this double rotation)
        {
            while (rotation < 0) rotation += 360.0;
            rotation %= 360.0;
            int rot = (int)Math.Round(rotation);

            if (rot != rotation && rot % 90 != 0)
            {
                throw new ArgumentException(Strings.Exception_notRotaion);
            }

            switch (rot)
            {
                case 0:
                    return Rotation.Rotate0;
                case 90:
                    return Rotation.Rotate90;
                case 180:
                    return Rotation.Rotate180;
                case 270:
                    return Rotation.Rotate270;
            }
            throw new ArgumentException(Strings.Exception_notRotaion);
        }

        public static float ToRadAngle(this Rotation rotation)
        {
            return (float) (ToAngle(rotation)*Math.PI/180d);
        }

        public static Vector2 Turn(this Vector2 point, Rotation rotation, Vector2 rotationPoint = new Vector2())
        {
            var tempVector = point - rotationPoint;
            for (double angle = 0, n = rotation.ToAngle(); angle < n; angle += 90.0)
            {
                tempVector = TurnLeft(tempVector);
            }
            return tempVector + rotationPoint;
        }

        public static Vector2 TurnBack(this Vector2 point, Rotation rotation, Vector2 rotationPoint = new Vector2())
        {
            var tempVector = point - rotationPoint;
            for (double angle = 0, n = rotation.ToAngle(); angle < n; angle += 90.0)
            {
                tempVector = TurnRight(tempVector);
            }
            return tempVector + rotationPoint;
        }

        private static Vector2 TurnLeft(Vector2 vector2)
        {
            return new Vector2(-vector2.Y, vector2.X);
        }

        private static Vector2 TurnRight(Vector2 vector2)
        {
            return new Vector2(vector2.Y, -vector2.X);
        }
    }
}