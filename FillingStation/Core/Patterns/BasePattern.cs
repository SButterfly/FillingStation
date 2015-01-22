using FillingStation.Core.Properties;

namespace FillingStation.Core.Patterns
{
    public abstract class BasePattern : IPattern
    {
        protected BasePattern(int width, int height)
        {
            Height = height;
            Width = width;
        }

        protected BasePattern(int width, int height, string imagePath)
            : this(width, height)
        {
            ImagePath = imagePath;
        }

        public int Height { get; private set; }
        public int Width { get; private set; }

        public string ImagePath { get; private set; }

        public virtual IProperty Property { get; protected set; }
    }

    public abstract class BasePattern<T> : BasePattern where T : IProperty, new()
    {
        protected BasePattern(int width, int height)
            : base(width, height)
        {
            base.Property = new T();
        }

        protected BasePattern(int width, int height, string imagePath)
            : base(width, height, imagePath)
        {
            base.Property = new T();
        }

        public new T Property
        {
            get { return (T) base.Property; } 
            protected set { base.Property = value; }
        }
    }
}
