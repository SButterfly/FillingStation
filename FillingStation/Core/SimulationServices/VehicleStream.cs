using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Core.Generators;
using FillingStation.Core.Models;
using FillingStation.Core.Vehicles;
using FillingStation.DAL;
using FillingStation.DAL.Models;
using FillingStation.Extensions;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.SimulationServices
{
    public class VehicleStream
    {
        private readonly IGenerator _generator;
        private readonly FSStateModel _stateModel;

        private readonly List<CarType> _carTypes;

        private TimeSpan _dt;
        private TimeSpan _lastCarSend;

        public VehicleStream(IGenerator generator, FSStateModel stateModel)
        {
            _generator = generator;
            _stateModel = stateModel;

            _carTypes = new CarTypeAccessor().All();

            _dt = new TimeSpan((long)generator.Next());
            _lastCarSend = new TimeSpan(0);
        }

        public IList<BaseVehicleType> GetNewCars(GameTime gameTime, IEnumerable<BaseVehicle> vehiclesOnField)
        {
            var resultList = new List<BaseVehicleType>();

            if (_stateModel.CurrentMoney >= _stateModel.LimitMoney && !vehiclesOnField.OfType<CashVehicle>().Any())
            {
                resultList.Add(new CasherType());
            }

            bool tankCheck = _stateModel.CurrentFuel92 <= _stateModel.CriticalFuel92 ||
                    _stateModel.CurrentFuel95 <= _stateModel.CriticalFuel95 ||
                    _stateModel.CurrentFuel98 <= _stateModel.CriticalFuel98 ||
                    _stateModel.CurrentFuelDiesel <= _stateModel.CriticalFuelDiesel;

            if (tankCheck && !vehiclesOnField.OfType<TankerVehicle>().Any())
            {
                if (_stateModel.CurrentFuel92 <= _stateModel.LowFuel92)
                {
                    resultList.Add(new TankerType(Fuel.A92));
                }
                if (_stateModel.CurrentFuel95 <= _stateModel.LowFuel95)
                {
                    resultList.Add(new TankerType(Fuel.A95));
                }
                if (_stateModel.CurrentFuel98 <= _stateModel.LowFuel98)
                {
                    resultList.Add(new TankerType(Fuel.A98));
                }
                if (_stateModel.CurrentFuelDiesel <= _stateModel.LowFuelDiesel)
                {
                    resultList.Add(new TankerType(Fuel.Diesel));
                }
            }

            var deltaTime = gameTime.TotalGameTime - _lastCarSend;

            if (_dt <= deltaTime)
            {
                long count = deltaTime.Ticks / _dt.Ticks;
                for (long i = 0; i < count; i++)
                {
                    resultList.Add(_carTypes.Random());
                }
                _lastCarSend = gameTime.TotalGameTime;
                _dt = new TimeSpan((long)_generator.Next());
            }

            return resultList;
        }
    }
}