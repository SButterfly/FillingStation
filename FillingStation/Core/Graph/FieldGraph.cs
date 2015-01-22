using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Core.Patterns;

namespace FillingStation.Core.Graph
{
    public class FieldGraph : Graph<IGameRoadPattern>
    {
        public override IGameRoadPattern StartPattern
        {
            get { return Objects.First(pattern => (pattern is EnterPattern)); }
        }
        public override IGameRoadPattern EndPattern
        {
            get { return Objects.First(pattern => (pattern is ExitPattern)); }
        }

        public IEnumerable<IGameRoadPattern> StartPatterns
        {
            get { return Objects.Where(pattern => (pattern is EnterPattern)); }
        }

        public IEnumerable<IGameRoadPattern> EndPatterns
        {
            get { return Objects.Where(pattern => (pattern is ExitPattern)); }
        }

        public void Merge(Graph<IGameRoadPattern> graph)
        {
            if (graph == null) throw new ArgumentNullException("graph");
            if (graph.StartPattern == null) throw new ArgumentException("graph.StartPattern could not be null");
            if (graph.EndPattern == null) throw new ArgumentException("graph.EndPattern could not be null");

            _graph.AddRange(graph._graph);
            
            var startNode = graph.GetNode(graph.StartPattern);
            var endNode = graph.GetNode(graph.EndPattern);

            foreach (var node in _graph)
            {
                if (startNode.Second.X == node.Second.X &&
                    startNode.Second.Y + 1 == node.Second.Y)
                {
                    node.Next.Add(startNode);
                }

                if (endNode.Second.X == node.Second.X &&
                    endNode.Second.Y + 1 == node.Second.Y)
                {
                    endNode.Next.Add(node);
                }
            }
        }
    }
}