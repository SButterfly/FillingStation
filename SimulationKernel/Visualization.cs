using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SimulationClassLibrary.Kernel;

namespace SimulationClassLibrary
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Visualization : GraphicsManager
    {
        private readonly SpriteBatch _spriteBatch;

        private XnaObject _backgroundField;
        private Texture2D _hideRectangle;
        private const float _hideWidth = 100f;
        private const float _hideHeight = 800f;

        public Visualization(Control parentControl, string contentDirectory = "Content")
            : base(parentControl)
        {
            Content.RootDirectory = contentDirectory;

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            float x = (Size.Width - _backgroundField.Size.Width) / 2f;
            float y = (Size.Height - _backgroundField.Size.Height) / 2f;

            _backgroundField.Position = new Vector2(x, y);
        }

        private XnaObject _simulationModel;
        public XnaObject SimulationModel
        {
            get { return _simulationModel; }
            set
            {
                if (!IsStarted)
                {
                    _simulationModel = value;
                }
                else
                {
                    throw new Exception("Couldn't set Simulation model during modelling");
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void LoadBackgroundField(Stream backgroundImage)
        {
            _backgroundField = new XnaObject(this);
            _backgroundField.LoadContent(backgroundImage);

            SizeChanged += OnSizeChanged;

            //Hack to redraw :)
            OnSizeChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _hideRectangle = new Texture2D(GraphicsDevice, (int)_hideWidth, (int)_hideHeight);

            Color[] data = new Color[(int)_hideWidth * (int)_hideHeight];
            for (int i = 0; i < data.Length; ++i) data[i] = new Color(204, 204, 204);
            _hideRectangle.SetData(data);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _simulationModel.Update(gameTime);
        }

        protected override void DrawBackground()
        {
            if (_backgroundField != null)
            {
                _spriteBatch.Begin();
                _backgroundField.Draw(_spriteBatch);
                _spriteBatch.End();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            //Отрисовываем автомобили
            _simulationModel.Draw(_spriteBatch, _backgroundField.Position);

            var position = _backgroundField.Position;

            _spriteBatch.Draw(_hideRectangle, new Vector2(position.X - _hideWidth, position.Y), Color.White);
            _spriteBatch.Draw(_hideRectangle, new Vector2(position.X + _backgroundField.Size.Width, position.Y), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
