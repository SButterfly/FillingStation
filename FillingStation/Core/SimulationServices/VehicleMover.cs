using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Core.Graph;
using FillingStation.Core.Pathes;
using FillingStation.Core.Patterns;
using FillingStation.Core.Vehicles;
using FillingStation.Extensions;
using FillingStation.Helpers;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.SimulationServices
{
    /// <summary>
    /// Sync between patterns and cars' position
    /// </summary>
    public class VehicleMover
    {
        #region Fields

        private class CurrentState
        {
            public BasePath Path { get; set; }
            public IGameRoadPattern Pattern { get; set; }
            public IGameRoadPattern NextPattern { get; set; }
            public IGameRoadPattern NextMovingPattern { get; set; }
        }

        private readonly VehicleAwaiter _vehicleAwaiter;
        private readonly PatternChooser _patternChooser;
        private readonly FieldGraph _modelGraph;

        private readonly Func<IEnumerable<BasePath>, BaseVehicle, BasePath> _pathChooser;

        private readonly Dictionary<BaseVehicle, CurrentState> _vehiclesOnField =
            new Dictionary<BaseVehicle, CurrentState>();

        #endregion

        #region Initialization

        public VehicleMover(FieldGraph modelGraph, VehicleAwaiter vehicleAwaiter,
           PatternChooser patternChooser)
        {
            if (patternChooser == null) throw new ArgumentNullException("patternChooser");
            if (vehicleAwaiter == null) throw new ArgumentNullException("vehicleAwaiter");

            _vehicleAwaiter = vehicleAwaiter;
            _modelGraph = modelGraph;
            _patternChooser = patternChooser;

            _pathChooser = (list, vehicle) => list.Random();
            vehicleAwaiter.StartWaiting += (sender, vehicle) =>
            {
                var columnPattern = GetPattern(vehicle) as ColumnPattern;
                if (columnPattern != null)
                    MessageFillingSubscribes(columnPattern, vehicle, FillingState.Start);
            };

            vehicleAwaiter.FinishWaiting += (sender, vehicle) =>
            {
                var columnPattern = GetPattern(vehicle) as ColumnPattern;
                if (columnPattern != null)
                    MessageFillingSubscribes(columnPattern, vehicle, FillingState.Finish);
            };
        }

        public VehicleMover(FieldGraph modelGraph, VehicleAwaiter vehicleAwaiter,
           PatternChooser patternChooser,
            Func<IEnumerable<BasePath>, BaseVehicle, BasePath> pathChooser)
        {
            if (patternChooser == null) throw new ArgumentNullException("patternChooser");
            if (pathChooser == null) throw new ArgumentNullException("pathChooser");
            if (vehicleAwaiter == null) throw new ArgumentNullException("vehicleAwaiter");

            _vehicleAwaiter = vehicleAwaiter;
            _modelGraph = modelGraph;
            _patternChooser = patternChooser;
            _pathChooser = pathChooser;
        }

        #endregion

        #region Properties

        public IEnumerable<BaseVehicle> Vehicles
        {
            get { return _vehiclesOnField.Keys; }
        }

        public IGameRoadPattern StartPattern
        {
            get { return _modelGraph.StartPattern; }
        }

        public IEnumerable<IGameRoadPattern> EndPatterns
        {
            get { return _modelGraph.EndPatterns; }
        }

        public FieldGraph Graph
        {
            get { return _modelGraph; }
        }

        public VehicleAwaiter VehicleAwaiter
        {
            get { return _vehicleAwaiter; }
        }

        public PatternChooser PatternChooser
        {
            get { return _patternChooser; }
        }

        #endregion

        #region Game methods

        public void UpdateVehicles(GameTime gameTime)
        {
            foreach (var vehicle in Vehicles)
            {
                UpdateVehicle(vehicle, gameTime);
            }
        }
        
        /// <summary>
        /// The main idea:
        /// All patterns has 3 points: enter, wait and exit.
        /// Cars update their's position to the nearest point.
        /// If car is on wait point, it will be probably wait
        /// When car is on exit point, it gets new pattern and find path to it;
        /// </summary>
        public void UpdateVehicle(BaseVehicle vehicle, GameTime gameTime)
        {
            if (gameTime.ElapsedGameTime.Ticks <= 3) return;

            if (!ContainsVehicle(vehicle)) throw new ArgumentException("Автомобиль не содержится в графе");

            if (IsOnPoint(vehicle, PointType.Enter))
            {
                MessageSubscribes(GetPattern(vehicle), vehicle);
            }

            if (IsOnPoint(vehicle, PointType.Wait))
            {
                var waitPercent = _vehicleAwaiter.Wait(vehicle, GetPattern(vehicle), ref gameTime);
                vehicle.WaitIndicator = waitPercent;
            }

            if (IsOverPoint(vehicle, PointType.Wait))
            {
                if (GetNextVehicle(vehicle) != null)
                {
                    vehicle.SetSpeed(GetNextVehicle(vehicle).Speed/2f);
                    UpdateToPoint(vehicle, PointType.Exit, ref gameTime);
                    return;
                }
            }

            if (IsOnPoint(vehicle, PointType.Exit))
            {
                var savePattern = GetPattern(vehicle);

                var currentPattern = GetNextPattern(vehicle);
                if (currentPattern == null)
                    return;

                SetPattern(vehicle, currentPattern);
                var nextPattern = SelectNextPattern(vehicle);

                BasePath path = null;

                if (nextPattern != null)
                {
                    path = SelectCurrentPath(vehicle, currentPattern, nextPattern);
                }
                else
                {
                    var previousPath = GetPath(vehicle);
                    var exitPoint = GetModelPoint(savePattern, previousPath.Exit);
                    foreach (var pp in currentPattern.Paths)
                    {
                        var enterPoint = GetModelPoint(currentPattern, pp.Enter);
                        if (exitPoint.IsNearBy(enterPoint))
                        {
                            path = pp;
                            break;
                        }
                    }
                }
                var position = _modelGraph[currentPattern];

                ClearStates(vehicle);

                SetPattern(vehicle, currentPattern);
                SetPath(vehicle, path);
                SetPosition(vehicle, position.ToVector2() + path.Enter);
                SetNextPattern(vehicle, nextPattern);

                UpdateVehicle(vehicle, gameTime);
                return;
            }

            SetVehicleSpeed(vehicle, gameTime);

            var nextPointType = IsOverPoint(vehicle, PointType.Wait) || IsOnPoint(vehicle, PointType.Wait)
                ? PointType.Exit
                : PointType.Wait;

            SetNextMovingPattern(vehicle, nextPointType == PointType.Wait ? null : GetNextPattern(vehicle));

            UpdateToPoint(vehicle, nextPointType, ref gameTime);

            UpdateVehicle(vehicle, gameTime);
        }

        private void SetVehicleSpeed(BaseVehicle vehicle, GameTime gameTime)
        {
            var nextVehicle = GetNextVehicle(vehicle);

            bool needToSlowDown = false;
            bool needToSpeedUp = true;

            if (!IsOverPoint(vehicle, PointType.Wait))
            {
                needToSlowDown |= WillVehicleCrashedIntoNextVehicle(vehicle);
                needToSlowDown |= VehicleAwaiter.WillWait(vehicle, GetPattern(vehicle));
            }

            if (IsOnPoint(vehicle, PointType.Wait))
            {
                if (nextVehicle != null && nextVehicle.WaitIndicator < 1e-5)
                {
                    vehicle.MaxSpeed = Math.Min(vehicle.MaxSpeed, nextVehicle.MaxSpeed - 0.00002f);
                }
            }

            if (!needToSpeedUp && !needToSlowDown && vehicle.Speed <= 0.02f)
            {
                throw new Exception("Smth went wrong speed is low; And no flag sets");
            }

            if (needToSlowDown) //means to slow down exactly to wait point
            {
                var point = GetPatternPoint(vehicle, GetPattern(vehicle));
                var path = GetPath(vehicle);
                var stopLength = path.GetRoad(point, path.Wait);

                if (stopLength >= 1e-8) //just skip when road is to small
                {
                    var vehicleSpeed = vehicle.Speed;
                    var time = gameTime.ElapsedGameTime.TotalSeconds;

                    var newSpeed = (float) (vehicleSpeed - time * vehicleSpeed * vehicleSpeed / (2 * stopLength));

                    vehicle.SetSpeed(Math.Max(0.002f, newSpeed), false);
                }
                return;
            }

            if (needToSpeedUp)
            {
                vehicle.SetSpeed(vehicle.MaxSpeed);
            }
        }

        private bool WillVehicleCrashedIntoNextVehicle(BaseVehicle vehicle)
        {
            var nextVehicle = GetNextVehicle(vehicle);
            return nextVehicle != null && vehicle.Speed > nextVehicle.Speed;
        }

        private IGameRoadPattern SelectNextPattern(BaseVehicle vehicle)
        {
            return _patternChooser.ChooseNextPattern(vehicle);
        }

        private BasePath SelectCurrentPath(BaseVehicle vehicle, IGameRoadPattern pattern, IGameRoadPattern nextPattern)
        {
            var currentPoint = GetModelPoint(pattern, GetPatternPoint(vehicle, pattern));

            var result = new List<BasePath>();
            foreach (var path in pattern.Paths)
            {
                var exitPoint = GetModelPoint(pattern, path.Exit);
                var enterPoint = GetModelPoint(pattern, path.Enter);
                if (currentPoint.IsNearBy(enterPoint, 1e-3))
                {
                    foreach (var nextPath in nextPattern.Paths)
                    {
                        var enterNextPoint = GetModelPoint(nextPattern, nextPath.Enter);
                        if (enterNextPoint.IsNearBy(exitPoint, 1e-3))
                        {
                            result.Add(path);
                            break;
                        }
                    }
                }
            }

            var resultPath = _pathChooser(result, vehicle);
            return resultPath;
        }

        public IEnumerable<BasePath> GetLinkPathes(IGameRoadPattern pattern, IGameRoadPattern nextPattern)
        {
            var result = new List<BasePath>();
            foreach (var path in pattern.Paths)
            {
                var exitPoint = GetModelPoint(pattern, path.Exit);
                foreach (var nextPath in nextPattern.Paths)
                {
                    var enterNextPoint = GetModelPoint(nextPattern, nextPath.Enter);
                    if (enterNextPoint.IsNearBy(exitPoint, 1e-3))
                    {
                        result.Add(path);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Point methods

        public Vector2 GetPatternPoint(BaseVehicle vehicle, IGameRoadPattern pattern)
        {
            var modelPosition = GetPosition(vehicle);
            var patternPosition = _modelGraph[pattern];

            return new Vector2(modelPosition.X - patternPosition.X, modelPosition.Y - patternPosition.Y);
        }

        private void SetPatternPoint(BaseVehicle vehicle, IGameRoadPattern pattern, Vector2 point)
        {
            var patternPosition = _modelGraph[pattern];

            SetPosition(vehicle, patternPosition.ToVector2() + point);
        }

        public Vector2 GetModelPoint(IGameRoadPattern pattern, Vector2 patternPoint)
        {
            var patternPosition = _modelGraph[pattern];

            return patternPosition.ToVector2() + patternPoint;
        }

        public bool IsOnPoint(BaseVehicle vehicle, PointType pointType)
        {
            var point = GetPath(vehicle).GetPoint(pointType);
            var currentPoint = GetPatternPoint(vehicle, GetPattern(vehicle));

            return point.IsNearBy(currentPoint, 1e-3);
        }

        public bool IsOverPoint(BaseVehicle vehicle, PointType pointType)
        {
            var path = GetPath(vehicle);

            var startPoint = path.GetPoint(PointType.Enter);
            var toPoint = path.GetPoint(pointType);

            var vehiclePoint = GetPatternPoint(vehicle, GetPattern(vehicle));

            var pointRoad = path.GetRoad(startPoint, toPoint);
            var vehicleRoad = path.GetRoad(startPoint, vehiclePoint);

            return vehicleRoad > pointRoad;
        }

        private void SetRotation(BaseVehicle vehicle, float rotation)
        {
            vehicle.Rotation = rotation;
        }

        public float GetRotation(BaseVehicle vehicle)
        {
            return vehicle.Rotation;
        }

        private void UpdateToPoint(BaseVehicle vehicle, PointType pointType, ref GameTime gameTime)
        {
            //Получаем позиции, связанные с автомобилем
            var path = GetPath(vehicle);
            var toPoint = path.GetPoint(pointType);
            var fromPoint = GetPatternPoint(vehicle, GetPattern(vehicle));

            //Получаем путь, который действительно может проехать автомобиль за пройденное время
            var road = vehicle.GetRoad((float)gameTime.ElapsedGameTime.TotalSeconds);
            var pointRoad = path.GetRoad(fromPoint, toPoint);

            //Находим путь, который он может проехать не превышая нашу точку
            float resultRoad = Math.Min(road, pointRoad);

            if (resultRoad < 0)
                throw new ArgumentException();

            if (resultRoad < 1e-7) //road is to small to continue update
            {
                gameTime = new GameTime(gameTime.TotalGameTime + gameTime.ElapsedGameTime, new TimeSpan());
                return;
            }

            var resultPoint = path.GetPoint(fromPoint, resultRoad);
            var rotation = path.GetTurn(resultPoint);

            //Обновляем текущие позиции
            SetPatternPoint(vehicle, GetPattern(vehicle), resultPoint);
            SetRotation(vehicle, rotation);
            var elapsedTime = new TimeSpan((long)(vehicle.GetElapsedTime(resultRoad) * TimeSpan.TicksPerSecond));

            if (gameTime.ElapsedGameTime - elapsedTime < new TimeSpan())
                throw new ArgumentException();

            gameTime = new GameTime(gameTime.TotalGameTime + elapsedTime, gameTime.ElapsedGameTime - elapsedTime);
        }

        #endregion

        #region Vehicles methods

        public void AddVehicle(BaseVehicle vehicle, IGameRoadPattern pattern = null)
        {
            if (ContainsVehicle(vehicle)) throw new ArgumentException("Current strategy has already contained this vehicle", "vehicle");

            _vehiclesOnField.Add(vehicle, new CurrentState());

            if (pattern == null) pattern = _patternChooser.ChooseStartPattern(vehicle);
            SetPattern(vehicle, pattern);

            var position = _modelGraph[pattern];
            SetPosition(vehicle, position.ToVector2() + pattern.Paths.First().Enter);

            var nextPattern = SelectNextPattern(vehicle);
            SetNextPattern(vehicle, nextPattern);

            var path = SelectCurrentPath(vehicle, pattern, nextPattern);
            SetPath(vehicle, path);
        }

        public void RemoveVehicle(BaseVehicle vehicle)
        {
            _vehiclesOnField.Remove(vehicle);
        }

        public bool ContainsVehicle(BaseVehicle vehicle)
        {
            return _vehiclesOnField.ContainsKey(vehicle);
        }

        public void RemoveLeftVehicles()
        {
            var list = Vehicles.Where(vehicle => IsOnPoint(vehicle, PointType.Exit) && EndPatterns.Any(pattern => pattern == GetPattern(vehicle))).ToList();

            foreach (var vehicle in list)
            {
                RemoveVehicle(vehicle);
            }
        }

        #endregion

        #region Vehicle methods

        private void ClearStates(BaseVehicle vehicle)
        {
            var value = _vehiclesOnField[vehicle];
            value.Pattern = null;
            value.Path = null;
            value.NextPattern = null;
        }

        public IGameRoadPattern GetPattern(BaseVehicle vehicle)
        {
            return _vehiclesOnField[vehicle].Pattern;
        }

        private void SetPattern(BaseVehicle vehicle, IGameRoadPattern pattern)
        {
            var node = _vehiclesOnField[vehicle];
            node.Pattern = pattern;
        }

        public IGameRoadPattern GetNextPattern(BaseVehicle vehicle)
        {
            return _vehiclesOnField[vehicle].NextPattern;
        }

        private void SetNextPattern(BaseVehicle vehicle, IGameRoadPattern nextPattern)
        {
            var node = _vehiclesOnField[vehicle];
            node.NextPattern = nextPattern;
        }

        public IGameRoadPattern GetNextMovingPattern(BaseVehicle vehicle)
        {
            return _vehiclesOnField[vehicle].NextMovingPattern;
        }

        private void SetNextMovingPattern(BaseVehicle vehicle, IGameRoadPattern pattern)
        {
            _vehiclesOnField[vehicle].NextMovingPattern = pattern;
        }

        public BasePath GetPath(BaseVehicle vehicle)
        {
            return _vehiclesOnField[vehicle].Path;
        }

        private void SetPath(BaseVehicle vehicle, BasePath path)
        {
            var node = _vehiclesOnField[vehicle];
            node.Path = path;
        }

        public Vector2 GetPosition(BaseVehicle vehicle)
        {
            return vehicle.Position;
        }

        private void SetPosition(BaseVehicle vehicle, Vector2 position)
        {
            vehicle.Position = position;
        }

        public BaseVehicle GetVechicle(IGameRoadPattern pattern)
        {
            return Vehicles.FirstOrDefault(baseVehicle => GetPattern(baseVehicle) == pattern);
        }

        public BaseVehicle GetNextVehicle(BaseVehicle vehicle)
        {
            var nextPattern = GetNextPattern(vehicle);
            return nextPattern == null ? null : GetVechicle(nextPattern);
        }

        #endregion

        #region Pattern methods

        public bool IsPatternFree(IGameRoadPattern pattern, BaseVehicle vehicle = null)
        {
            bool isFree = Vehicles.Select(GetPattern).All(vehiclePattern => pattern != vehiclePattern);
            if (isFree && vehicle != null)
            {
                isFree &= Vehicles.Where(v => v != vehicle).Select(GetNextMovingPattern).All(pat => pat != GetNextPattern(vehicle));
            }
            return isFree;
        }

        #endregion

        #region Subscribe Methods

        private readonly Dictionary<IGameRoadPattern, IList<Action<IGameRoadPattern, BaseVehicle>>> _subscribeActions = new Dictionary<IGameRoadPattern, IList<Action<IGameRoadPattern, BaseVehicle>>>();
        
        public void SubscribeEnterPattern(IGameRoadPattern pattern, Action<IGameRoadPattern, BaseVehicle> callback)
        {
            IList<Action<IGameRoadPattern, BaseVehicle>> list;
            if (_subscribeActions.ContainsKey(pattern))
            {
                list = _subscribeActions[pattern];
            }
            else
            {
                list = new List<Action<IGameRoadPattern, BaseVehicle>>(1);
                _subscribeActions.Add(pattern, list);
            }
            list.Add(callback);
        }

        public void UnsubscribeEnterPattern(IGameRoadPattern pattern, Action<IGameRoadPattern, BaseVehicle> callback)
        {
            var list = _subscribeActions[pattern];
            list.Remove(callback);
            if (list.Count == 0)
            {
                _subscribeActions.Remove(pattern);
            }
        }

        public void MessageSubscribes(IGameRoadPattern pattern, BaseVehicle vehicle)
        {
            if (!_subscribeActions.ContainsKey(pattern)) return;

            foreach (var subscribeAction in _subscribeActions[pattern])
            {
                subscribeAction(pattern, vehicle);
            }
        }

        public enum FillingState
        {
            Start,
            Finish
        }

        private readonly Dictionary<ColumnPattern, IList<Action<ColumnPattern, BaseVehicle>>> _subscribeFillingStartActions = new Dictionary<ColumnPattern, IList<Action<ColumnPattern, BaseVehicle>>>();
        private readonly Dictionary<ColumnPattern, IList<Action<ColumnPattern, BaseVehicle>>> _subscribeFillingFinishActions = new Dictionary<ColumnPattern, IList<Action<ColumnPattern, BaseVehicle>>>();

        public void SubscribeFilling(ColumnPattern pattern, FillingState state, Action<ColumnPattern, BaseVehicle> callback)
        {
            var dictionary = state == FillingState.Start
                ? _subscribeFillingStartActions
                : _subscribeFillingFinishActions;

            IList<Action<ColumnPattern, BaseVehicle>> list;
            if (dictionary.ContainsKey(pattern))
            {
                list = dictionary[pattern];
            }
            else
            {
                list = new List<Action<ColumnPattern, BaseVehicle>>(1);
                dictionary.Add(pattern, list);
            }
            list.Add(callback);
        }

        public void UnsubscribeFilling(ColumnPattern pattern, FillingState state, Action<ColumnPattern, BaseVehicle> callback)
        {
            var dictionary = state == FillingState.Start
                ? _subscribeFillingStartActions
                : _subscribeFillingFinishActions;

            var list = dictionary[pattern];
            list.Remove(callback);
            if (list.Count == 0)
            {
                dictionary.Remove(pattern);
            }
        }

        public void MessageFillingSubscribes(ColumnPattern pattern, BaseVehicle vehicle, FillingState state)
        {
            var dictionary = state == FillingState.Start
                ? _subscribeFillingStartActions
                : _subscribeFillingFinishActions;

            if (!dictionary.ContainsKey(pattern)) return;

            foreach (var subscribeAction in dictionary[pattern])
            {
                subscribeAction(pattern, vehicle);
            }
        }

        #endregion
    }
}