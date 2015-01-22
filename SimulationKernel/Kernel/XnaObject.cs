using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimulationClassLibrary.Kernel
{
    public class XnaObject
    {
        private readonly string _fileName;

        public GraphicsManager GraphicsManager { get; private set; }

        public XnaObject(GraphicsManager graphicsManager, string fileName = null)
        {
            _fileName = fileName;
            GraphicsManager = graphicsManager;
            Position = Vector2.Zero;
            Color = Color.White;
            Rotation = 0f;
            Scale = 1f;
            Origin = Vector2.Zero;
        }

        #region Properties

        public virtual Texture2D Texture { get; set; }
        public virtual Color Color { get; set; }
        public virtual Vector2 Position { get; set; }
        public virtual Rectangle Size { get; set; }
        public virtual float Rotation { get; set; }
        public virtual float Scale { get; set; }
        public virtual Vector2 Origin { get; set; }

        #endregion

        #region Game methods

        public virtual void LoadContent()
        {
            Texture = LoadTexture2D(_fileName);
            Size = Texture.Bounds;
        }

        public virtual void LoadContent(Stream stream)
        {
            Texture = LoadTexture2D(stream);
            Size = Texture.Bounds;
        }

        protected virtual Texture2D LoadTexture2D(string fileName)
        {
            try
            {
                return GraphicsManager.Content.Load<Texture2D>(fileName);
            }
            catch
            { }
            using (var fileStream = new FileStream(GraphicsManager.Content.RootDirectory +"/"+ fileName, FileMode.Open))
            {
                return LoadTexture2D(fileStream);
            }
        }

        protected virtual Texture2D LoadTexture2D(Stream textureStream)
        {
            return Texture2D.FromStream(GraphicsManager.GraphicsDevice, textureStream);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Position);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Texture, position, Size, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 offset)
        {
            spriteBatch.Draw(Texture, position + offset, Size, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }

        #endregion
    }
}