using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Core.Generators;
using FillingStation.Core.Models;
using FillingStation.Core.Vehicles;
using FillingStation.DAL;
using FillingStation.DAL.Models;
using FillingStation.Extensions;
using FillingStation.Helpers;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.SimulationServices
{
    public class VehicleStream
    {
        private readonly IGenerator _generator;
        private readonly FSStateModel _stateModel;

        private List<CarType> _car92Types;
        private List<CarType> _car95Types;
        private List<CarType> _car98Types;
        private List<CarType> _carDieselTypes;
        private int[] _carProbabilityDensity;

        private Random rand;

        private TimeSpan _dt;
        private TimeSpan _lastCarSend;

        public VehicleStream(IGenerator generator, FSStateModel stateModel)
        {
            _generator = generator;
            _stateModel = stateModel;

            List<CarType> _carTypes = new CarTypeAccessor().All();
            _car92Types = new List<CarType>();
            _car95Types = new List<CarType>();
            _car98Types = new List<CarType>();
            _carDieselTypes = new List<CarType>();
            foreach (CarType carType in _carTypes)
            {
                if (carType.Fuel == Fuel.A92)
                {
                    _car92Types.Add(carType); 
                }
                if (carType.Fuel == Fuel.A95)
                {
                    _car95Types.Add(carType);
                }
                if (carType.Fuel == Fuel.A98)
                {
                    _car98Types.Add(carType);
                }
                if (carType.Fuel == Fuel.Diesel)
                {
                    _carDieselTypes.Add(carType);
                }
            }

            _carProbabilityDensity = new int[3];
            IList<FuelConsumptionModel> fuelData = new FuelModelAccessor().All();
            foreach (FuelConsumptionModel model in fuelData)
            {
                if (model.Fuel == Fuel.A92)
                {
                    _carProbabilityDensity[0] = model.CarPercentage;
                }
                if (model.Fuel == Fuel.A95)
                {
                    _carProbabilityDensity[1] = _carProbabilityDensity[0] + model.CarPercentage;
                }
                if (model.Fuel == Fuel.A98)
                {
                    _carProbabilityDensity[2] = _carProbabilityDensity[1] + model.CarPercentage;
                }
            }

            rand = Randomizer.GetInstance().Random;

            _dt = new TimeSpan(0);
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
            while (_dt <= deltaTime)
            {
                int temp = rand.Next(1, 101);
                if (temp > 0 && temp <= _carProbabilityDensity[0])
                {
                    resultList.Add(_car92Types.Random());
                }
                if (temp > _carProbabilityDensity[0] && temp <= _carProbabilityDensity[1])
                {
                    resultList.Add(_car95Types.Random());
                }
                if (temp > _carProbabilityDensity[1] && temp <= _carProbabilityDensity[2])
                {
                    resultList.Add(_car98Types.Random());
                }
                if (temp > _carProbabilityDensity[2] && temp <= 100)
                {
                    resultList.Add(_carDieselTypes.Random());
                }

                _lastCarSend += _dt;
                deltaTime -= _dt;
                _dt = new TimeSpan((long)_generator.Next());
            }

            return resultList;
        }
    }
}