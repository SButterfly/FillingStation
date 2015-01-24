using System.Collections.Generic;
using FillingStation.Core.Pathes;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    #warning This class is not for patterns palette
    public sealed class EnterPattern : BasePattern, IGameRoadPattern
    {
        public EnterPattern()
            : base(1, 1)
        {
            var firstLine = new LinePath(new Vector2(1f, 0.5f), new Vector2(0f, 0.5f));

            Paths = new BasePath[] {
                firstLine
            };
        }

        public IList<BasePath> Paths { get; private set; }
    }
}