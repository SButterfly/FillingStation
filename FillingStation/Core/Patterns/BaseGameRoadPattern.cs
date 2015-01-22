using System.Collections.Generic;
using FillingStation.Core.Pathes;
using FillingStation.Core.Properties;

namespace FillingStation.Core.Patterns
{
    public abstract class BaseGameRoadPattern<T> : BasePattern<T>, IGameRoadPattern where T : IProperty, new()
    {
        public virtual IList<BasePath> Paths { get; protected set; }

        protected BaseGameRoadPattern(int width, int height) 
            : base(width, height)
        {
        }

        protected BaseGameRoadPattern(int width, int height, string imagePath) 
            : base(width, height, imagePath)
        {
        }
    }
}