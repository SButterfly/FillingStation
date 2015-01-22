using System;
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

        protected BaseVehicle(GraphicsManager graphicsManager, BaseVehicleType vehicleType)
            : base(graphicsManager, vehicleType.ImagePath)
        {
            VehicleType = vehicleType;
            var rand = Randomizer.GetInstance();

            Speed = 1.5f + (float)rand.Random.NextDouble();
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

        public virtual float Speed { get; set; }

        public override Rectangle Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                Origin = new Vector2(value.Width / 2f, value.Height / 2f);
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