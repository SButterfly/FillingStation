using System;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using FillingStation.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SimulationClassLibrary.Kernel;

namespace FillingStation.Core.Vehicles
{
    public abstract class BaseVehicle : XnaObject
    {
        private Texture2D _waitProgressBackground;
        private Texture2D _waitProgressStatus;
        private Rectangle _waitProgressSize;
        private Vector2 _waitProgressOrigin;
        private float _speed;

        protected BaseVehicle(GraphicsManager graphicsManager, BaseVehicleType vehicleType)
            : base(graphicsManager, vehicleType.ImagePath)
        {
            VehicleType = vehicleType;
            var rand = Randomizer.GetInstance();

            MaxSpeed = 1.5f + (float)rand.Random.NextDouble();
            Speed = MaxSpeed;

            Acceleration = 0.1f;
            Origin = Vector2.Zero;

            _waitProgressSize = new Rectangle(0, 0, 40, 13);
            _waitProgressOrigin = new Vector2(_waitProgressSize.Width/2f, _waitProgressSize.Height/2f);
        }

        public virtual void LoadContent(Texture2D texture)
        {
            Texture = texture;
            Size = texture.Bounds;

            _waitProgressBackground = new Texture2D(GraphicsManager.GraphicsDevice, 1, 1);
            _waitProgressBackground.SetData(new Color[] { Color.Blue });

            _waitProgressStatus = new Texture2D(GraphicsManager.GraphicsDevice, 1, 1);
            _waitProgressStatus.SetData(new Color[] { Color.LightBlue });
        }

        public virtual BaseVehicleType VehicleType { get; private set; }

        public virtual float Speed
        {
            get { return _speed; }
            private set
            {
                if (Double.IsNaN(value))
                    throw new ArgumentException("Value is NaN");

                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Speed must be not negative");

                _speed = value;
            }
        }

        public virtual float MaxSpeed { get; set; }

        public virtual float Acceleration { get; set; }

        public override Rectangle Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                Origin = new Vector2(value.Width / 2f, value.Height / 2f);
            }
        }

        public virtual void SetSpeed(float newSpeed, bool useAcceleration = true)
        {
            if (Math.Sign(Speed - newSpeed) == 0)
                return;
            
            if (useAcceleration)
            {
                var ds = Math.Min(Math.Abs(Speed - newSpeed), Acceleration);
                int sign = Math.Sign(Speed - newSpeed);
                Speed -= sign*ds;
            }
            else
            {
                Speed = newSpeed;
            }
        }

        #region Game Methods

        public float GetRoad(float elapsedTime)
        {
            return elapsedTime*Speed;
        }

        public float GetElapsedTime(float road)
        {
            return road / Speed;
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public double WaitIndicator { get; set; }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 offset)
        {
            base.Draw(spriteBatch, position, offset);
            if (WaitIndicator >= 1e-5)
            {
                spriteBatch.Draw(_waitProgressBackground, position + offset - _waitProgressOrigin, _waitProgressSize, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                var currentProgress = new Rectangle(0, 0, (int)((_waitProgressSize.Width - 5) * WaitIndicator), _waitProgressSize.Height - 5);
                spriteBatch.Draw(_waitProgressStatus, position + offset - _waitProgressOrigin + new Vector2(2.5f, 2.5f), currentProgress, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("BaseVehicle <speed={0}, VehicleType={1}", Speed, VehicleType == null? "<null>" : VehicleType.ToString());
        }
    }
}