using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Core.Graph;
using FillingStation.Core.Models;
using FillingStation.Core.Patterns;
using FillingStation.Core.Properties;
using FillingStation.Core.Vehicles;
using FillingStation.Extensions;

namespace FillingStation.Core.SimulationServices
{
    public class PatternChooser
    {
        private readonly Stack<IGameRoadPattern> _stack = new Stack<IGameRoadPattern>();

        private const int _nullRoadKey = 1000;
        private const int _containsRoadKey = 1000 - 1;

        public PatternChooser(FSModel fsModel, FieldModel fielModel, FieldGraph fieldGraph)
        {
            FSModel = fsModel;
            FielModel = fielModel;
            FieldGraph = fieldGraph;
        }

        public FSModel FSModel { get; private set; }
        public FieldModel FielModel { get; set; }
        public FieldGraph FieldGraph { get; set; }
        public VehicleMover VehicleMover { get { return FielModel.VehicleMover; } }

        public IGameRoadPattern ChooseStartPattern(BaseVehicle vehicle)
        {
            if (vehicle is TankerVehicle || vehicle is CashVehicle)
            {
                var nextPattern = FieldGraph.StartPattern;
                float minY = FieldGraph[nextPattern].Y;
                foreach (var pattern in FieldGraph.StartPatterns)
                {
                    var pos = VehicleMover.Graph[pattern];
                    if (pos.Y < minY)
                    {
                        minY = pos.Y;
                        nextPattern = pattern;
                    }
                }
                return nextPattern;
            }

            return FieldGraph.StartPatterns.Random();
        }

        public IGameRoadPattern ChooseNextPattern(BaseVehicle vehicle)
        {
            var nextPatterns = FieldGraph.Next(VehicleMover.GetPattern(vehicle));

            if (!nextPatterns.Any()) return null;
            if (nextPatterns.Count() == 1) return nextPatterns.First();

            var currentPattern = VehicleMover.GetPattern(vehicle);

            if (vehicle.VehicleType is CasherType)
            {
                var nearPattern = FSModel.GetNearestGameRoadPattern<CashBoxPattern>();
                var nextPattern = GetRoad(currentPattern, VehicleMover, gamePattern => gamePattern == nearPattern);
                if (nextPattern != null)
                {
                    return nextPattern;
                }
            }

            if (vehicle.VehicleType is TankerType)
            {
                var tankerType = (TankerType)vehicle.VehicleType;
                var tankPattern = FSModel.Patterns.OfType<TankPattern>().
                    First(pattern => ((TankProperty) pattern.Property).Fuel == tankerType.FuelType);
                var nextPattern = GetRoad(currentPattern, VehicleMover, gamePattern => gamePattern == tankPattern);
                if (nextPattern != null)
                {
                    return nextPattern;
                }
            }

            if (vehicle.VehicleType is CarType)
            {
                var nextPattern = GetRoad(currentPattern, VehicleMover, gamePattern => gamePattern is ColumnPattern);
                if (nextPattern != null)
                {
                    return nextPattern;
                }
            }

            return nextPatterns.Random();
        }

        private IGameRoadPattern GetRoad(IGameRoadPattern pattern, VehicleMover synchronizer, Func<IGameRoadPattern, bool> exitFunc)
        {
            Dictionary<IGameRoadPattern, int> dictionary = new Dictionary<IGameRoadPattern, int>();
            foreach (var nextPattern in synchronizer.Graph.Next(pattern))
            {
                int freePatterns = BusyRoad(nextPattern, synchronizer, exitFunc);

                if (freePatterns != _nullRoadKey && freePatterns != _containsRoadKey)
                    dictionary.Add(nextPattern, freePatterns);
            }

            if (dictionary.Count == 0) return null;

            var increase = dictionary.Values.OrderBy(value => value);
            var min = increase.FirstOrDefault();

            return dictionary.Where(kv => kv.Value == min).Select(kv => kv.Key).Random();
        }

        private int BusyRoad(IGameRoadPattern pattern, VehicleMover synchronizer, Func<IGameRoadPattern, bool> exitFunc)
        {
            if (pattern == null) return _nullRoadKey;
            if (_stack.Contains(pattern)) return _containsRoadKey;
            if (exitFunc(pattern)) 
                return !synchronizer.IsPatternFree(pattern) ? 1 : 0;

            int busyPatterns = _nullRoadKey;

            if (!exitFunc(pattern))
            {
                _stack.Push(pattern);

                foreach (var gamePattern in synchronizer.Graph.Next(pattern))
                {
                    int result = BusyRoad(gamePattern, synchronizer, exitFunc);
                    busyPatterns = Math.Min(result, busyPatterns);
                }

                _stack.Pop();
            }

            if (!synchronizer.IsPatternFree(pattern) && busyPatterns != _nullRoadKey && busyPatterns != _containsRoadKey)
            {
                busyPatterns++;
            }

            return busyPatterns;
        }
    }
}