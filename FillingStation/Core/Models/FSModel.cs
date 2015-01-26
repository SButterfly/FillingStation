using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using FillingStation.Core.Graph;
using FillingStation.Core.Patterns;
using FillingStation.Core.Properties;
using FillingStation.Core.Vehicles;
using FillingStation.DAL.Models;
using FillingStation.Extensions;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Core.Models
{
    public class FSModel
    {
        #region Fields

        private readonly Dictionary<IPattern, Point> _dictionary = new Dictionary<IPattern, Point>();

        #endregion

        #region Intitialization

        public FSModel(int width, int height)
        {
            Height = height;
            Width = width;
        }

        #endregion

        #region Properties

        public int Height { get; private set; }
        public int Width { get; private set; }

        public IEnumerable<IPattern> Patterns { get { return _dictionary.Keys; } }

        #endregion

        #region Events

        public event EventHandler Changed;

        private void Property_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var ev = Changed;
            if (ev != null)
            {
                ev(this, null);
            }
        }

        #endregion

        #region Methods

        public void Add(IPattern pattern, Point point)
        {
            if (!CanPlace(pattern, point)) 
                throw new ArgumentException(Strings.Exception_pattern_place + point, "point");

            _dictionary.Add(pattern, point);

            pattern.Property.PropertyChanged += Property_PropertyChanged;
            Property_PropertyChanged(this, null);
        }

        public void Remove(IPattern pattern)
        {
            bool success = _dictionary.Remove(pattern);
            if (success)
            {
                pattern.Property.PropertyChanged -= Property_PropertyChanged;
                Property_PropertyChanged(this, null);
            }
        }

        public bool Contains(IPattern pattern)
        {
            return _dictionary.ContainsKey(pattern);
        }

        public bool CanPlace(IPattern pattern, Point point)
        {
            for (int i = 0; i < pattern.Width; i++)
            {
                for (int j = 0; j < pattern.Height; j++)
                {
                    if (HasUsed(point.X + i, point.Y + j))
                        return false;
                }
            }

            return true;
        }

        private bool HasUsed(int x, int y)
        {
            foreach (var keyValue in _dictionary)
            {
                for (int i = 0; i < keyValue.Key.Width; i++)
                {
                    for (int j = 0; j < keyValue.Key.Height; j++)
                    {
                        if (keyValue.Value.X + i == x &&
                            keyValue.Value.Y + j == y)
                            return true;
                    }
                }
            }
            return false;
        }

        public IPattern Get(Point point)
        {
            int x = point.X;
            int y = point.Y;
            foreach (var keyValue in _dictionary)
            {
                for (int i = 0; i < keyValue.Key.Width; i++)
                {
                    for (int j = 0; j < keyValue.Key.Height; j++)
                    {
                        if (keyValue.Value.X + i == x && keyValue.Value.Y + j == y)
                            return keyValue.Key;
                    }
                }
            }

            return null;
        }

        public Point Get(IPattern pattern)
        {
            return _dictionary[pattern];
        }

        public IGameRoadPattern GetNearestGameRoadPattern<T>() where T : IPattern
        {
            var pattern = Patterns.FirstOrDefault(pat => pat is T);
            if (pattern == null)
                throw new ArgumentException("There are no patterns of type " + typeof(T));

            return GetNearestGameRoadPattern(pattern);
        }

        public IGameRoadPattern GetNearestGameRoadPattern(IPattern pattern)
        {
            if (!Contains(pattern))
                throw new ArgumentException("Model doesn't contain this pattern.");

            var point = Get(pattern);
            var nearPoints = new List<Point>();

            for (int i = 0; i < pattern.Width; i++)
            {
                nearPoints.Add(new Point(point.X + i, point.Y - 1));
                nearPoints.Add(new Point(point.X + i, point.Y + pattern.Height));
            }

            for (int i = 0; i < pattern.Height; i++)
            {
                nearPoints.Add(new Point(point.X - 1, point.Y + i));
                nearPoints.Add(new Point(point.X + pattern.Width, point.Y + i));
            }

            return nearPoints.Select(nearPoint => _dictionary.FirstOrDefault(kv => kv.Value == nearPoint).Key).OfType<IGameRoadPattern>().FirstOrDefault();
        }

        public bool IsCorrect()
        {
            var infoTableList = Patterns.OfType<InfoTablePattern>().ToArray();
            var cashBoxList = Patterns.OfType<CashBoxPattern>().ToArray();
            var roadInList = Patterns.OfType<RoadInPattern>().ToArray();
            var roadOutList = Patterns.OfType<RoadOutPattern>().ToArray();
            var rezList = Patterns.OfType<TankPattern>().ToArray();
            var kolList = Patterns.OfType<ColumnPattern>().ToArray();

            if (infoTableList.Count() != 1)
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectnessFormat, 1, Strings.InfoTable));
            }

            if (cashBoxList.Count() != 1)
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectnessFormat, 1, Strings.CashBox));
            }
            var cashBoxProperty = cashBoxList.First().Property;
            if (!(500000 <= cashBoxProperty.CashBoxLimit && cashBoxProperty.CashBoxLimit <= 1500000))
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectness_CashRangeFormat, 500000, 1500000));
            }
            if (GetNearestGameRoadPattern<CashBoxPattern>() == null)
            {
                throw new Exception(Strings.Exception_cashNearRoad);
            }

            if (roadInList.Count() != 1)
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectnessFormat, 1, Strings.RoadIn));
            }
            if (Get(roadInList.First()).Y != Height - 1)
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectness_PatternMainRoadFormat, Strings.RoadIn));
            }

            if (roadOutList.Count() != 1)
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectnessFormat, 1, Strings.RoadOut));
            }
            if (Get(roadOutList.First()).Y != Height - 1)
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectness_PatternMainRoadFormat, Strings.RoadOut));
            }

            var roadInPoint = Get(roadInList.First());
            var roadOutPoint = Get(roadOutList.First());
            if (roadInPoint.X < roadOutPoint.X)
            {
                throw new Exception(Strings.Exception_FSModelCorrectness_RoadInOut);
            }

            if (kolList.Count() < 0)
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectnessFormat, 1, Strings.Column));
            }

            var a92 = rezList.FirstOrDefault(rez => rez.Property.Fuel == Fuel.A92);
            var a95 = rezList.FirstOrDefault(rez => rez.Property.Fuel == Fuel.A95);
            var a98 = rezList.FirstOrDefault(rez => rez.Property.Fuel == Fuel.A98);
            var diesel = rezList.FirstOrDefault(rez => rez.Property.Fuel == Fuel.Diesel);

            if (a92 == null || a95 == null || a98 == null || diesel == null)
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectnessFormat, 4, Strings.Tank) 
                    + "\n" + Strings.Exception_FSModelCorrectness_TankRange);
            }

            var isCorrect = IsTankPropertyCorrect(a92.Property) &&
                            IsTankPropertyCorrect(a95.Property) &&
                            IsTankPropertyCorrect(a98.Property) &&
                            IsTankPropertyCorrect(diesel.Property);
            FSGraph graph;
            try
            {
                graph = GenerateGraph();
            }
            catch (Exception e)
            {
                throw new Exception(Strings.Exception_FSIsIncorrect);
            }

            var areAllRoadPatternsUsed = Patterns.OfType<IGameRoadPattern>().All(pattern => graph.Contains(pattern));
            if (!areAllRoadPatternsUsed)
            {
                throw new Exception(Strings.Exception_FSModelCorrectness_PatternsNotUsed);
            }

            return true;
        }

        private static bool IsTankPropertyCorrect(TankProperty property)
        {
            var limit = property.TankLimit;
            var low = property.LowTankLimit;
            var critical = property.CriticalTankLimit;

            if (!(0 < limit && limit <= 50000))
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectness_TankLimitRangeFormat, FuelExtensions.ToString(property.Fuel), 0, 50000));
            }

            if (!(0 < low && low <= limit))
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectness_LowTankLimitFormat, FuelExtensions.ToString(property.Fuel), 0));
            }

            if (!(0 < critical && critical <= low))
            {
                throw new Exception(string.Format(Strings.Exception_FSModelCorrectness_CriticalTankLimitFormat, FuelExtensions.ToString(property.Fuel), 0));
            }

            return true;
        }

        #endregion

        #region Graph Generation

        private FSGraph _graph;
        private Stack<IGameRoadPattern> _patternsInStack;

        public FSGraph GenerateGraph()
        {
            _graph = new FSGraph();
            _patternsInStack = new Stack<IGameRoadPattern>();

            var startPattern = Patterns.OfType<RoadInPattern>().First();
            SetConnections(startPattern, Get(startPattern), new Point());

            if (_graph.Objects.Any(patternsInGraph => !Patterns.OfType<IGameRoadPattern>().Contains(patternsInGraph)))
            {
                throw new Exception("Ошибка при построении топологии. Не все дороги связаны.");
            }
		
            var resultGraph = _graph;
            _graph = null;
            _patternsInStack = null;
            return resultGraph;
        }

        private bool SetConnections(IGameRoadPattern pattern, Point point, Point previousPoint)
        {
            if (pattern == null) throw new Exception("Дороги связаны некорректно");

            if (_patternsInStack.Contains(pattern))
                return false;

            if (_graph.Contains(pattern))
                return true;

            if (pattern is RoadOutPattern)
            {
                _graph.Add(pattern, point);
                return true;
            }

            _patternsInStack.Push(pattern);

            bool canSetConnection = false;

            if (pattern is RoadTPattern)
            {
                var roadTPattern = (RoadTPattern) pattern;
                var nextPoints = NextPoints(point, previousPoint, roadTPattern);

                var nextPattern = Get(nextPoints.First) as IGameRoadPattern;
                var anotherNextPattern = Get(nextPoints.Second) as IGameRoadPattern;

                if (HasConnection(pattern, point, nextPattern, nextPoints.First) || HasConnection(pattern, point, anotherNextPattern, nextPoints.Second))
                {
                    var canSetFirstConnection = SetConnections(nextPattern, nextPoints.First, point);
                    if (canSetFirstConnection)
                    {
                        _graph.Add(pattern, point);
                        _graph.Bind(pattern, nextPattern);
                    }

                    var canSetSecondConnection = SetConnections(anotherNextPattern, nextPoints.Second, point);
                    if (canSetSecondConnection)
                    {
                        if (!_graph.Contains(pattern))
                            _graph.Add(pattern, point);
                        _graph.Bind(pattern, anotherNextPattern);
                    }

                    canSetConnection = canSetFirstConnection || canSetSecondConnection;
                }
            }
            else
            {
                var nextPoint = NextPoint(point, previousPoint, pattern);
                var nextPattern = Get(nextPoint) as IGameRoadPattern;

                if (HasConnection(pattern, point, nextPattern, nextPoint))
                {
                    canSetConnection = SetConnections(nextPattern, nextPoint, point);
                    if (canSetConnection)
                    {
                        _graph.Add(pattern, point);
                        _graph.Bind(pattern, nextPattern);
                    }
                }
            }

            _patternsInStack.Pop();
            return canSetConnection;
        }

        private bool HasConnection(IGameRoadPattern currentPattern, Point currentPoint, IGameRoadPattern nextPattern, Point nextPoint)
        {
            #region Current Pattern - RoadIn

            if (currentPattern is RoadInPattern)
            {
                if (nextPattern is RoadPattern || nextPattern is ColumnPattern)
                {
                    var turnProperty = (ITurnProperty)nextPattern.Property;
                    return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180;
                }
                if (nextPattern is RoadTurnPattern)
                {
                    var turnProperty = (ITurnProperty)nextPattern.Property;
                    return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate270;
                }
                if (nextPattern is RoadTPattern)
                {
                    var turnProperty = (ITurnProperty)nextPattern.Property;
                    return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                }
                if (nextPattern is TankPattern || nextPattern is RoadOutPattern)
                {
                    return true;
                }
            }

            #endregion

            #region Current Pattern - Rez

            if (currentPattern is TankPattern)
            {
                if (nextPattern is RoadPattern || nextPattern is ColumnPattern)
                {
                    var turnProperty = (ITurnProperty)nextPattern.Property;
                    return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180;
                }
                if (nextPattern is RoadTurnPattern)
                {
                    var turnProperty = (ITurnProperty)nextPattern.Property;
                    if (nextPoint.Y == currentPoint.Y - 1)
                    {
                        return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate270;
                    }
                    if (nextPoint.Y == currentPoint.Y + 1)
                    {
                        return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180;
                    }
                }
                if (nextPattern is RoadTPattern)
                {
                    var turnProperty = (ITurnProperty)nextPattern.Property;
                    if (nextPoint.Y == currentPoint.Y - 1)
                    {
                        return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                    }
                    if (nextPoint.Y == currentPoint.Y + 1)
                    {
                        return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180;
                    }
                    return false;
                }
                if (nextPattern is TankPattern || nextPattern is RoadOutPattern)
                {
                    return true;
                }
            }

            #endregion

            #region Current Pattern - Road or Kol

            if (currentPattern is RoadPattern || currentPattern is ColumnPattern)
            {
                var currentTurnProperty = (ITurnProperty)currentPattern.Property;
                if (currentTurnProperty.Angle == Rotation.Rotate0 || currentTurnProperty.Angle == Rotation.Rotate180)
                {
                    if (nextPattern is RoadPattern || nextPattern is ColumnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180;
                    }
                    if (nextPattern is RoadTurnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180;
                        }
                    }
                    if (nextPattern is RoadTPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180;
                        }
                        return false;
                    }
                    if (nextPattern is TankPattern || nextPattern is RoadOutPattern)
                    {
                        return true;
                    }
                }
                if (currentTurnProperty.Angle == Rotation.Rotate90 || currentTurnProperty.Angle == Rotation.Rotate270)
                {
                    if (nextPattern is RoadPattern || nextPattern is ColumnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate270;
                    }
                    if (nextPattern is RoadTurnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90;
                        }
                    }
                    if (nextPattern is RoadTPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        return false;
                    }
                    if (nextPattern is TankPattern || nextPattern is RoadOutPattern)
                    {
                        return false;
                    }
                }       
            }

            #endregion

            #region Current Pattern - RoadTurn

            if (currentPattern is RoadTurnPattern)
            {
                var currentTurnProperty = (ITurnProperty)currentPattern.Property;
                if (currentTurnProperty.Angle == Rotation.Rotate0)
                {
                    if (nextPattern is RoadPattern || nextPattern is ColumnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180;
                        }
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate270;
                        }
                    }
                    if (nextPattern is RoadTurnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180;
                        }
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                    }
                    if (nextPattern is RoadTPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180;
                        }
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        return false;
                    }
                    if (nextPattern is TankPattern || nextPattern is RoadOutPattern)
                    {
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return true;
                        }
                    }
                }
                if (currentTurnProperty.Angle == Rotation.Rotate90)
                {
                    if (nextPattern is RoadPattern || nextPattern is ColumnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180;
                        }
                    }
                    if (nextPattern is RoadTurnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate270;
                        }
                    }
                    if (nextPattern is RoadTPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        return false;
                    }
                    if (nextPattern is TankPattern || nextPattern is RoadOutPattern)
                    {
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return true;
                        }
                    }
                }
                if (currentTurnProperty.Angle == Rotation.Rotate180)
                {
                    if (nextPattern is RoadPattern || nextPattern is ColumnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180;
                        }
                    }
                    if (nextPattern is RoadTurnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90;
                        }
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate270;
                        }
                    }
                    if (nextPattern is RoadTPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        return false;
                    }
                    if (nextPattern is TankPattern || nextPattern is RoadOutPattern)
                    {
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return true;
                        }
                    }
                }
                if (currentTurnProperty.Angle == Rotation.Rotate270)
                {
                    if (nextPattern is RoadPattern || nextPattern is ColumnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate180;
                        }
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate270;
                        }
                    }
                    if (nextPattern is RoadTurnPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180;
                        }
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90;
                        }
                    }
                    if (nextPattern is RoadTPattern)
                    {
                        ITurnProperty turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate180;
                        }
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle == Rotation.Rotate0 || turnProperty.Angle == Rotation.Rotate90 || turnProperty.Angle == Rotation.Rotate270;
                        }
                        return false;
                    }
                    if (nextPattern is TankPattern || nextPattern is RoadOutPattern)
                    {
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return true;
                        }
                    }
                }
            }

            #endregion

            #region Current Pattern - RoadT

            if (currentPattern is RoadTPattern)
            {
                if (nextPattern is TankPattern || nextPattern is RoadPattern || nextPattern is ColumnPattern || nextPattern is RoadTurnPattern)
                {
                    return HasConnection(nextPattern, nextPoint, currentPattern, currentPoint);
                }
                var currentTurnProperty = (ITurnProperty)currentPattern.Property;
                if (currentTurnProperty.Angle == Rotation.Rotate0)
                {
                    if (nextPattern is RoadTPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate90;
                        }
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate270;
                        }
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate0;
                        }
                        return false;
                    }
                    if (nextPattern is RoadOutPattern)
                    {
                        if (nextPoint.Y == currentPoint.Y + 1 || nextPoint.Y == currentPoint.Y - 1)
                        {
                            return true;
                        }
                    }
                }
                if (currentTurnProperty.Angle == Rotation.Rotate90)
                {
                    if (nextPattern is RoadTPattern)
                    {
                        ITurnProperty turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate90;
                        }
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate0;
                        }
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate180;
                        }
                        else return false;
                    }
                    if (nextPattern is RoadOutPattern)
                    {
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return true;
                        }
                    }
                }
                if (currentTurnProperty.Angle == Rotation.Rotate180)
                {
                    if (nextPattern is RoadTPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y - 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate90;
                        }
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate270;
                        }
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate180;
                        }
                        return false;
                    }
                    if (nextPattern is RoadOutPattern)
                    {
                        if (nextPoint.Y == currentPoint.Y + 1 || nextPoint.Y == currentPoint.Y - 1)
                        {
                            return true;
                        }
                    }
                }
                if (currentTurnProperty.Angle == Rotation.Rotate270)
                {
                    if (nextPattern is RoadTPattern)
                    {
                        var turnProperty = (ITurnProperty)nextPattern.Property;
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate270;
                        }
                        if (nextPoint.X == currentPoint.X - 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate0;
                        }
                        if (nextPoint.X == currentPoint.X + 1)
                        {
                            return turnProperty.Angle != Rotation.Rotate180;
                        }
                        return false;
                    }
                    if (nextPattern is RoadOutPattern)
                    {
                        if (nextPoint.Y == currentPoint.Y + 1)
                        {
                            return true;
                        }
                    }
                }
            }

            #endregion

            return false;
        }

        private Pair<Point, Point> NextPoints(Point point, Point previousPoint, RoadTPattern pattern)
        {
            var first = new Point();
            var second = new Point();

            var turnProperty = (ITurnProperty)pattern.Property;
            if (turnProperty.Angle == Rotation.Rotate0)
            {
                if (previousPoint.X == point.X - 1)
                {
                    first.X = point.X;
                    first.Y = point.Y - 1;
                    second.X = point.X;
                    second.Y = point.Y + 1;
                }
                if (previousPoint.Y == point.Y + 1)
                {
                    first.X = point.X - 1;
                    first.Y = point.Y;
                    second.X = point.X;
                    second.Y = point.Y - 1;
                }
                if (previousPoint.Y == point.Y - 1)
                {
                    first.X = point.X - 1;
                    first.Y = point.Y;
                    second.X = point.X;
                    second.Y = point.Y + 1;
                }
            }
            if (turnProperty.Angle == Rotation.Rotate90)
            {
                if (previousPoint.X == point.X - 1)
                {
                    first.X = point.X;
                    first.Y = point.Y - 1;
                    second.X = point.X + 1;
                    second.Y = point.Y;
                }
                if (previousPoint.Y == point.Y - 1)
                {
                    first.X = point.X - 1;
                    first.Y = point.Y;
                    second.X = point.X + 1;
                    second.Y = point.Y;
                }
                if (previousPoint.X == point.X + 1)
                {
                    first.X = point.X;
                    first.Y = point.Y - 1;
                    second.X = point.X - 1;
                    second.Y = point.Y;
                }
            }
            if (turnProperty.Angle == Rotation.Rotate180)
            {
                if (previousPoint.X == point.X + 1)
                {
                    first.X = point.X;
                    first.Y = point.Y - 1;
                    second.X = point.X;
                    second.Y = point.Y + 1;
                }
                if (previousPoint.Y == point.Y + 1)
                {
                    first.X = point.X + 1;
                    first.Y = point.Y;
                    second.X = point.X;
                    second.Y = point.Y - 1;
                }
                if (previousPoint.Y == point.Y - 1)
                {
                    first.X = point.X + 1;
                    first.Y = point.Y;
                    second.X = point.X;
                    second.Y = point.Y + 1;
                }
            }
            if (turnProperty.Angle == Rotation.Rotate270)
            {
                if (previousPoint.X == point.X - 1)
                {
                    first.X = point.X;
                    first.Y = point.Y + 1;
                    second.X = point.X + 1;
                    second.Y = point.Y;
                }
                if (previousPoint.Y == point.Y + 1)
                {
                    first.X = point.X - 1;
                    first.Y = point.Y;
                    second.X = point.X + 1;
                    second.Y = point.Y;
                }
                if (previousPoint.X == point.X + 1)
                {
                    first.X = point.X;
                    first.Y = point.Y + 1;
                    second.X = point.X - 1;
                    second.Y = point.Y;
                }
            }
            return new Pair<Point, Point>(first, second);
        }

        private Point NextPoint(Point point, Point previousPoint, IGameRoadPattern pattern)
        {
            if (pattern is RoadTPattern)
                throw new Exception("Can't find only one point for RoadTPattern. Use NextPoints instead");

            var nextPoint = new Point();

            if (pattern is RoadInPattern)
            {
                nextPoint.X = point.X;
                nextPoint.Y = point.Y - 1;
            }
            if (pattern is RoadPattern || pattern is ColumnPattern || pattern is TankPattern)
            {
                nextPoint.X = point.X + (point.X - previousPoint.X);
                nextPoint.Y = point.Y + (point.Y - previousPoint.Y);
            }
            if (pattern is RoadTurnPattern)
            {
                var turnProperty = (ITurnProperty)pattern.Property;
                if (turnProperty.Angle == Rotation.Rotate0)
                {
                    if (previousPoint.Y == point.Y)
                    {
                        nextPoint.X = point.X;
                        nextPoint.Y = point.Y + 1;
                    }
                    if (previousPoint.X == point.X)
                    {
                        nextPoint.X = point.X - 1;
                        nextPoint.Y = point.Y;
                    }
                }
                if (turnProperty.Angle == Rotation.Rotate90)
                {
                    if (previousPoint.Y == point.Y)
                    {
                        nextPoint.X = point.X;
                        nextPoint.Y = point.Y - 1;
                    }
                    if (previousPoint.X == point.X)
                    {
                        nextPoint.X = point.X - 1;
                        nextPoint.Y = point.Y;
                    }
                }
                if (turnProperty.Angle == Rotation.Rotate180)
                {
                    if (previousPoint.Y == point.Y)
                    {
                        nextPoint.X = point.X;
                        nextPoint.Y = point.Y - 1;
                    }
                    if (previousPoint.X == point.X)
                    {
                        nextPoint.X = point.X + 1;
                        nextPoint.Y = point.Y;
                    }
                }
                if (turnProperty.Angle == Rotation.Rotate270)
                {
                    if (previousPoint.Y == point.Y)
                    {
                        nextPoint.X = point.X;
                        nextPoint.Y = point.Y + 1;
                    }
                    if (previousPoint.X == point.X)
                    {
                        nextPoint.X = point.X + 1;
                        nextPoint.Y = point.Y;
                    }
                }
            }
            return nextPoint;
        }

        #endregion
    }
}
