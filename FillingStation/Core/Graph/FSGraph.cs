using System.Linq;
using FillingStation.Core.Patterns;

namespace FillingStation.Core.Graph
{
    public class FSGraph : Graph<IGameRoadPattern>
    {
        public override IGameRoadPattern StartPattern
        {
            get { return Objects.First(pattern => (pattern is RoadInPattern)); }
        }

        public override IGameRoadPattern EndPattern
        {
            get { return Objects.First(pattern => (pattern is RoadOutPattern)); }
        }
    }
}