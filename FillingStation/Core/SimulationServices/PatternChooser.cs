using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Core.Graph;
using FillingStation.Core.Models;
using FillingStation.Core.Patterns;
using FillingStation.Core.Vehicles;
using FillingStation.Extensions;

namespace FillingStation.Core.SimulationServices
{
    public class PatternChooser
    {
        private readonly Stack<IGameRoadPattern> _stack = new Stack<IGameRoadPattern>();

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

            var count = nextPatterns.Count();
            if (count <= 1) return nextPatterns.FirstOrDefault();

            var currentPattern = VehicleMover.GetPattern(vehicle);

            if (vehicle.VehicleType is CasherType)
            {
                var nearPattern = FSModel.GetNearestGameRoadPattern<CashBoxPattern>();
                var nextPattern = GetRoad(vehicle, currentPattern, VehicleMover, gamePattern => gamePattern == nearPattern);
                if (nextPattern != null)
                {
                    return nextPattern;
                }
            }

            var tankerType = vehicle.VehicleType as TankerType;
            if (tankerType != null)
            {
                var tankPattern = FSModel.Patterns.OfType<TankPattern>().
                    First(pattern => pattern.Property.Fuel == tankerType.FuelType);
                var nextPattern = GetRoad(vehicle, currentPattern, VehicleMover, gamePattern => gamePattern == tankPattern);
                if (nextPattern != null)
                {
                    return nextPattern;
                }
            }

            if (vehicle.VehicleType is CarType)
            {
                var nextPattern = GetRoad(vehicle, currentPattern, VehicleMover, gamePattern => gamePattern is ColumnPattern);
                if (nextPattern != null)
                {
                    return nextPattern;
                }
            }

            return nextPatterns.Random();
        }

        private IGameRoadPattern GetRoad(BaseVehicle vehicle, IGameRoadPattern pattern, VehicleMover synchronizer, Func<IGameRoadPattern, bool> exitFunc)
        {
            var dictionary = new Dictionary<IGameRoadPattern, double>();
            foreach (var nextPattern in synchronizer.Graph.Next(pattern))
            {
                double? timeToPattern = GetTimeToPattern(vehicle, nextPattern, synchronizer, exitFunc);

                if (timeToPattern != null)
                    dictionary.Add(nextPattern, timeToPattern.Value);
            }

            double minimum = Double.MaxValue;
            IGameRoadPattern resultPattern = null;
            foreach (var kv in dictionary)
            {
                if (kv.Value < minimum)
                {
                    minimum = kv.Value;
                    resultPattern = kv.Key;
                }
            }

            return resultPattern;
        }

        private double? GetTimeToPattern(BaseVehicle vehicle, IGameRoadPattern pattern, VehicleMover synchronizer, Func<IGameRoadPattern, bool> exitFunc)
        {
            if (pattern == null || _stack.Contains(pattern)) return null;

            if (exitFunc(pattern))
            {
                var vehicleOnPattern = synchronizer.GetVechicle(pattern);
                return vehicleOnPattern != null ? VehicleAwaiter.GetWaitingTime(vehicleOnPattern) : 0;
            }

            double? minTime = null;
            IGameRoadPattern nextPattern = null;

            if (!exitFunc(pattern))
            {
                _stack.Push(pattern);

                foreach (var gamePattern in synchronizer.Graph.Next(pattern))
                {
                    double? timeToPattern = GetTimeToPattern(vehicle, gamePattern, synchronizer, exitFunc);
                    if (timeToPattern.HasValue && (minTime == null || timeToPattern.Value < minTime.Value))
                    {
                        minTime = timeToPattern;
                        nextPattern = gamePattern;
                    }
                }

                _stack.Pop();
            }

            if (minTime != null)
            {
                var path = synchronizer.GetLinkPathes(pattern, nextPattern).First();
                var vehicleOnPattern = synchronizer.GetVechicle(pattern);
                minTime += path.Length/vehicle.Speed + (vehicleOnPattern != null ? VehicleAwaiter.GetWaitingTime(synchronizer.GetVechicle(pattern)) : 0);
            }

            return minTime;
        }
    }
}