using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FillingStation.Core.Graph
{
    public class Node<TFirst, TSecond>
    {
        public Node()
        {
            Next = new List<Node<TFirst, TSecond>>();
        } 

        public Node(TFirst first, TSecond second)
            : this()
        {
            First = first;
            Second = second;
        }

        public TFirst First { get; set; }
        public TSecond Second { get; set; }

        public IList<Node<TFirst, TSecond>> Next { get; set; }
    }

    public abstract class Graph<T>
    {
        #region Fields

        protected internal readonly List<Node<T, Point>> _graph;

        #endregion

        #region Initialization

        protected Graph()
        {
            _graph = new List<Node<T, Point>>();
        }

        protected Graph(IEnumerable<Node<T, Point>> graph)
        {
            _graph = graph.ToList();
        }

        #endregion

        #region Properties

        public abstract T StartPattern { get; }
        public abstract T EndPattern { get; }

        public Point this[T obj]
        {
            get { return GetNode(obj).Second; }
            set { GetNode(obj).Second = value; }
        }

        public IEnumerable<T> Objects
        {
            get { return _graph.Select(node => node.First); }
        }

        public IEnumerable<Point> Vectors
        {
            get { return _graph.Select(node => node.Second); }
        }

        #endregion

        #region Methods

        public void Add(T obj, Point position)
        {
            _graph.Add(new Node<T, Point>(obj, position));
        }

        public void Add(T baseObj, T bindObj, Point position)
        {
            Add(bindObj, position);
            Bind(baseObj, bindObj);
        }

        public void Bind(T baseObj, T bindObj)
        {
            var node = GetNode(baseObj);
            var newNode = GetNode(bindObj);
            node.Next.Add(newNode);
        }

        public void DeleteBind(T baseObj, T bindObj)
        {
            var node = GetNode(baseObj);
            var newNode = GetNode(bindObj);
            node.Next.Remove(newNode);
        }

        public void Remove(T obj)
        {
            var removeObj = _graph.FirstOrDefault(nodeVector2 => nodeVector2.First.Equals(obj));
            _graph.Remove(removeObj);

            foreach (var o in Objects)
            {
                DeleteBind(o, obj);
            }
        }

        public bool IsBinded(T baseObj, T bindObj)
        {
            var node = GetNode(baseObj);
            var newNode = GetNode(bindObj);
            return node != null && newNode != null && node.Next.Contains(newNode);
        }

        public bool Contains(T obj)
        {
            return GetNode(obj) != null;
        }

        public IEnumerable<T> Next(T obj)
        {
            return GetNode(obj).Next.Select(node => node.First);
        }

        protected internal virtual Node<T, Point> GetNode(T obj)
        {
            return _graph.FirstOrDefault(nodeVector2 => nodeVector2.First.Equals(obj));
        }

        #endregion
    }
}