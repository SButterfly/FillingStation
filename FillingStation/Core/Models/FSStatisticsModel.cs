using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FillingStation.Annotations;
using FillingStation.Core.Patterns;
using FillingStation.Core.SimulationServices;
using FillingStation.Core.Vehicles;
using FillingStation.DAL;
using FillingStation.DAL.Models;
using FillingStation.Helpers;

namespace FillingStation.Core.Models
{
    public class FSStatisticsModel : INotifyPropertyChanged
    {
        private readonly Func<TimeSpan> _modelTimeFunc;

        private readonly Dictionary<CarVehicle, TimeSpan> _vehiclesOnFS = new Dictionary<CarVehicle, TimeSpan>();
        private readonly Dictionary<CarVehicle, TimeSpan> _fillingVehicles = new Dictionary<CarVehicle, TimeSpan>();

        private readonly AverageDouble _carsToFS = new AverageDouble();
        private readonly AverageTimeSpan _carsOnFS = new AverageTimeSpan();
        private readonly AverageTimeSpan _carsFilling = new AverageTimeSpan();
        private readonly AverageDouble _carsFillingTank = new AverageDouble();
        private readonly AverageDouble _carsFillingCash = new AverageDouble();

        private readonly AverageTimeSpan _tank92Visits = new AverageTimeSpan();
        private readonly AverageTimeSpan _tank95Visits = new AverageTimeSpan();
        private readonly AverageTimeSpan _tank98Visits = new AverageTimeSpan();
        private readonly AverageTimeSpan _tankdieselVisits = new AverageTimeSpan();
        private readonly AverageTimeSpan _casherVisits = new AverageTimeSpan();

        private TimeSpan _lastTank92Visit = new TimeSpan();
        private TimeSpan _lastTank95Visit = new TimeSpan();
        private TimeSpan _lastTank98Visit = new TimeSpan();
        private TimeSpan _lastTankdieselVisit = new TimeSpan();
        private TimeSpan _lastCasherVisit = new TimeSpan();

        private FSStatisticsModel()
        {   
        }

        public FSStatisticsModel(VehicleMover vehicleMover, Func<TimeSpan> modelTimeFunc)
        {
            if (modelTimeFunc == null) throw new ArgumentNullException("modelTimeFunc");

            VehicleMover = vehicleMover;
            _modelTimeFunc = modelTimeFunc;

            SubscribeCarsToFS(vehicleMover);
            SubscribeAVGCarTime(vehicleMover);
            SubscribeFillingCarTime(vehicleMover);
        }

        #region Subscribe methods

        private void SubscribeCarsToFS(VehicleMover vehicleMover)
        {
            var graph = vehicleMover.Graph;

            var firstEnter = graph.StartPatterns.First();
            var secondEnter = graph.StartPatterns.First(pattern => pattern != firstEnter);

            var toFS = graph[firstEnter].Y < graph[secondEnter].Y ? firstEnter : secondEnter;
            var toExit = graph[firstEnter].Y < graph[secondEnter].Y ? secondEnter : firstEnter;

            vehicleMover.SubscribeEnterPattern(toFS, (pattern, vehicle) =>
            {
                if (vehicle is CarVehicle)
                {
                    _carsToFS.Add(1d, 1d);
                    OnPropertyChanged("CarsToFS");
                }
                else
                {
                    AverageTimeSpan averageTimeSpan = null;
                    TimeSpan lastVisit = new TimeSpan();
                    Action<TimeSpan> lastTimeSetter = null;
                    string propertyToUpdate = null;

                    if (vehicle is TankerVehicle && ((TankerVehicle) vehicle).VehicleType.FuelType == Fuel.A92)
                    {
                        averageTimeSpan = _tank92Visits;
                        lastVisit = _lastTank92Visit;
                        lastTimeSetter = span => { _lastTank92Visit = span; };
                        propertyToUpdate = "AVGTank92VisitTime";
                    }
                    if (vehicle is TankerVehicle && ((TankerVehicle)vehicle).VehicleType.FuelType == Fuel.A95)
                    {
                        averageTimeSpan = _tank95Visits;
                        lastVisit = _lastTank95Visit;
                        lastTimeSetter = span => { _lastTank95Visit = span; };
                        propertyToUpdate = "AVGTank95VisitTime";
                    }
                    if (vehicle is TankerVehicle && ((TankerVehicle)vehicle).VehicleType.FuelType == Fuel.A98)
                    {
                        averageTimeSpan = _tank98Visits;
                        lastVisit = _lastTank98Visit;
                        lastTimeSetter = span => { _lastTank98Visit = span; };
                        propertyToUpdate = "AVGTank98VisitTime";
                    }
                    if (vehicle is TankerVehicle && ((TankerVehicle)vehicle).VehicleType.FuelType == Fuel.Diesel)
                    {
                        averageTimeSpan = _tankdieselVisits;
                        lastVisit = _lastTankdieselVisit;
                        lastTimeSetter = span => { _lastTankdieselVisit = span; };
                        propertyToUpdate = "AVGTankDieselVisitTime";
                    }
                    if (vehicle is CashVehicle)
                    {
                        averageTimeSpan = _casherVisits;
                        lastVisit = _lastCasherVisit;
                        lastTimeSetter = span => { _lastCasherVisit = span; };
                        propertyToUpdate = "AVGCasherVisitTime";
                    }

                    if (averageTimeSpan != null && propertyToUpdate != null && lastTimeSetter != null)
                    {
                        var time = ModelTime;
                        averageTimeSpan.Add(time - lastVisit, 1);
                        lastTimeSetter(time);
                        OnPropertyChanged(propertyToUpdate);
                    }
                }
            });

            vehicleMover.SubscribeEnterPattern(toExit, (pattern, vehicle) =>
            {
                if (vehicle is CarVehicle)
                {
                    _carsToFS.Add(0d, 1d);
                    OnPropertyChanged("CarsToFS");
                }
            });
        }

        private void SubscribeAVGCarTime(VehicleMover vehicleMover)
        {
            var roadIn = vehicleMover.Graph.Objects.OfType<RoadInPattern>().First();
            var roadOut = vehicleMover.Graph.Objects.OfType<RoadOutPattern>().First();

            vehicleMover.SubscribeEnterPattern(roadIn, (pattern, vehicle) =>
            {
                var car = vehicle as CarVehicle;
                if (car != null && !_vehiclesOnFS.ContainsKey(car))
                {
                    _vehiclesOnFS.Add(car, ModelTime);
                }
            });
            vehicleMover.SubscribeEnterPattern(roadOut, (pattern, vehicle) =>
            {
                var car = vehicle as CarVehicle;
                if (car != null && _vehiclesOnFS.ContainsKey(car))
                {
                    var time = _vehiclesOnFS[car];
                    _carsOnFS.Add(ModelTime - time, 1);
                    OnPropertyChanged("AVGCarOnFSTime");

                    _vehiclesOnFS.Remove(car);
                }
            });
        }

        private void SubscribeFillingCarTime(VehicleMover vehicleMover)
        {
            var columnPatterns = vehicleMover.Graph.Objects.OfType<ColumnPattern>();
            foreach (var columnPattern in columnPatterns)
            {
                vehicleMover.SubscribeFilling(columnPattern, VehicleMover.FillingState.Start, Vehicle_OnStartFilling);
                vehicleMover.SubscribeFilling(columnPattern, VehicleMover.FillingState.Finish, Vehicle_OnFinishFilling);
            }   
        }

        private void Vehicle_OnStartFilling(ColumnPattern columnPattern, BaseVehicle baseVehicle)
        {
            var car = baseVehicle as CarVehicle;
            if (car != null && !_fillingVehicles.ContainsKey(car))
            {
                _fillingVehicles.Add(car, ModelTime);
            }
        }

        private void Vehicle_OnFinishFilling(ColumnPattern columnPattern, BaseVehicle baseVehicle)
        {
            var car = baseVehicle as CarVehicle;
            if (car != null && _fillingVehicles.ContainsKey(car))
            {
                var startTime = _fillingVehicles[car];
                _carsFilling.Add(ModelTime - startTime, 1);
                OnPropertyChanged("AVGCarFillingTime");

                var fillingTank = car.VehicleType.TankVolume;
                _carsFillingTank.Add(fillingTank, 1);
                OnPropertyChanged("AVGCarFillingTank");

                var price = fillingTank*GetFuelPrice(car.VehicleType.Fuel);
                _carsFillingCash.Add(price, 1);
                OnPropertyChanged("AVGCarFillingCash");

                _fillingVehicles.Remove(car);
            }
        }

        #endregion

        #region Properties

        public VehicleMover VehicleMover { get; private set; }

        public TimeSpan ModelTime
        {
            get { return _modelTimeFunc == null ? new TimeSpan() : _modelTimeFunc(); }
        }

        /// <summary>
        /// An percent of cars, which turn to FS
        /// </summary>
        public double CarsToFS
        {
            get { return _carsToFS.AVG*100d; }
        }

        /// <summary>
        /// An average time, car spending on FS
        /// </summary>
        public TimeSpan AVGCarOnFSTime
        {
            get { return _carsOnFS.AVG; }
        }

        /// <summary>
        /// An average filling time
        /// </summary>
        public TimeSpan AVGCarFillingTime
        {
            get { return _carsFilling.AVG; }
        }

        /// <summary>
        /// An average filling tank
        /// </summary>
        public double AVGCarFillingTank
        {
            get { return _carsFillingTank.AVG; }
        }

        /// <summary>
        /// An average filling cash
        /// </summary>
        public double AVGCarFillingCash
        {
            get { return _carsFillingCash.AVG; }
        }

        public TimeSpan AVGTank92VisitTime
        {
            get { return _tank92Visits.AVG; }
        }
        public TimeSpan AVGTank95VisitTime
        {
            get { return _tank95Visits.AVG; }
        }
        public TimeSpan AVGTank98VisitTime
        {
            get { return _tank98Visits.AVG; }
        }
        public TimeSpan AVGTankDieselVisitTime
        {
            get { return _tankdieselVisits.AVG; }
        }
        public TimeSpan AVGCasherVisitTime
        {
            get { return _casherVisits.AVG; }
        }

        #endregion

        #region Methods

        private Dictionary<Fuel, double> _fuelToPrice; 
        private double GetFuelPrice(Fuel fuel)
        {
            if (_fuelToPrice == null)
            {
                _fuelToPrice = new Dictionary<Fuel, double>(4);

                var allFuelTypes = new FuelModelAccessor().All();
                foreach (var fuelType in allFuelTypes)
                {
                    _fuelToPrice.Add(fuelType.Fuel, fuelType.Price);
                }
            }

            return _fuelToPrice[fuel];
        }

        public static FSStatisticsModel Empty
        {
            get
            {
                return new FSStatisticsModel();
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}