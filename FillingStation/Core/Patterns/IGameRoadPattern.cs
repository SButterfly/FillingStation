using System.Collections.Generic;
using FillingStation.Core.Pathes;

namespace FillingStation.Core.Patterns
{
    public interface IGameRoadPattern : IPattern
    {
        IList<BasePath> Paths { get; }
    }
}