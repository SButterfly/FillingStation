using System;
using System.Linq;
using System.Windows.Media.Imaging;
using FillingStation.Core.Properties;
using FillingStation.Extensions;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Pathes
{
    public enum PointType
    {
        Enter,
        Wait,
        Exit
    }

    public abstract class BasePath
    {
        #region Initialization

        protected BasePath() {}
        protected BasePath(Vector2 rotationPoint)
        {
            RotationPoint = rotationPoint;
        }

        #endregion

        #region Properties

        public Vector2 Enter { get { return NormalEnter.Turn(Rotation, RotationPoint); } }
        public Vector2 Wait { get { return NormalWait.Turn(Rotation, RotationPoint); } }
        public Vector2 Exit { get { return NormalExit.Turn(Rotation, RotationPoint); } }

        public Vector2 RotationPoint { get; private set; }
        public Rotation Rotation { get; private set; }

        public float Length { get { return GetRoad(Enter, Exit); } }

        #endregion

        #region Abstract methods

        protected abstract Vector2 NormalEnter { get; }
        protected abstract Vector2 NormalWait { get; }
        protected abstract Vector2 NormalExit { get; }

        protected abstract BasePath GetNormalInverted();
        protected abstract Vector2 GetNormalPoint(Vector2 startPoint, float road);
        protected abstract float GetNormalRoad(Vector2 fromPoint, Vector2 toPoint);
        protected abstract float GetNormalTurn(Vector2 rotationPoint);

        #endregion

        #region Methods

        public void BindToTurnProperty(ITurnProperty turnProperty)
        {
            if (turnProperty == null)
                throw new ArgumentNullException("turnProperty");

            var properties = turnProperty.GetType().GetProperties();
            var property = properties.FirstOrDefault(info => info.PropertyType == typeof (Rotation));

            turnProperty.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == property.Name)
                {
                    Rotation = turnProperty.Angle;
                }
            };
        }

        public Vector2 GetPoint(PointType pointType)
        {
            if (pointType == PointType.Enter) return Enter;
            if (pointType == PointType.Wait) return Wait;
            if (pointType == PointType.Exit) return Exit;

            return Vector2.Zero;
        }

        public Vector2 GetPoint(Vector2 startPoint, float road)
        {
            var newStartPoint = startPoint.TurnBack(Rotation, RotationPoint);
            var point = GetNormalPoint(newStartPoint, road);
            var resultPoint = point.Turn(Rotation, RotationPoint);
            return resultPoint;
        }

        public float GetRoad(Vector2 fromPoint, Vector2 toPoint)
        {
            fromPoint = fromPoint.TurnBack(Rotation, RotationPoint);
            toPoint = toPoint.TurnBack(Rotation, RotationPoint);
            return GetNormalRoad(fromPoint, toPoint);
        }

        public float GetTurn(Vector2 rotationPoint)
        {
            rotationPoint = rotationPoint.TurnBack(Rotation, RotationPoint);
            var rotation = GetNormalTurn(rotationPoint);
            rotation += Rotation.ToRadAngle();
            return rotation;
        }

        public BasePath Inverted()
        {
            var path = GetNormalInverted();
            path.Rotation = Rotation;
            return path;
        }

        public override string ToString()
        {
            return base.ToString().Replace("FillingStation.Core.Pathes.", "") + string.Format(" [Enter{0};Wait{1};Exit{2};]", Enter, Wait, Exit);
        }

        #endregion
    }
}