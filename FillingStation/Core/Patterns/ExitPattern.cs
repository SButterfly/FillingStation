using System.Collections.Generic;
using FillingStation.Core.Pathes;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.Patterns
{
    public sealed class ExitPattern : BasePattern, IGameRoadPattern
    {
        public ExitPattern()
            : base(1, 1)
        {
            var secondLine = new LinePath(new Vector2(1f, 0.5f), new Vector2(0f, 0.5f));

            Paths = new BasePath[] {
                secondLine
            };
        }

        public IList<BasePath> Paths { get; private set; }
    }
}