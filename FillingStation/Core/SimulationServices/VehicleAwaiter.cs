using System;
using System.Collections.Generic;
using FillingStation.Core.Models;
using FillingStation.Core.Patterns;
using FillingStation.Core.Properties;
using FillingStation.Core.Vehicles;
using FillingStation.DAL;
using FillingStation.DAL.Models;
using Microsoft.Xna.Framework;

namespace FillingStation.Core.SimulationServices
{
    public class VehicleAwaiter
    {
        private readonly FSStateModel _stateModel;
        private readonly HashSet<BaseVehicle> _waitingVehicles = new HashSet<BaseVehicle>();   

        private const double EPS = 1e-5;

        private const double _fillingSpeed = 50;
        private const double _refillingSpeed = 1000;
        private const double _cashingSpeed = 50000;

        private bool _canFill92;
        private bool _canFill95;
        private bool _canFill98;
        private bool _canFillDiesel;

        public VehicleAwaiter(FSModel model, FSStateModel stateModel)
        {
            FSModel = model;
            _stateModel = stateModel;

            var allFuelTypes = new FuelModelAccessor().All();
            foreach (var fuelType in allFuelTypes)
            {
                if (fuelType.Fuel == Fuel.A92)
                {
                    Fuel92Price = fuelType.Price;
                }
                if (fuelType.Fuel == Fuel.A95)
                {
                    Fuel95Price = fuelType.Price;
                }
                if (fuelType.Fuel == Fuel.A98)
                {
                    Fuel98Price = fuelType.Price;
                }
                if (fuelType.Fuel == Fuel.Diesel)
                {
                    FuelDieselPrice = fuelType.Price;
                }
            }
            _canFill92 = true;
            _canFill95 = true;
            _canFill98 = true;
            _canFillDiesel = true;
        }

        public FSModel FSModel { get; set; }

        public double Fuel92Price { get; private set; }
        public double Fuel95Price { get; private set; }
        public double Fuel98Price { get; private set; }
        public double FuelDieselPrice { get; private set; }

        public double Wait(BaseVehicle vehicle, IGameRoadPattern pattern, ref GameTime gameTime)
        {
            double result = 0d;
            if (vehicle is CarVehicle)
            {
                var car = (CarVehicle)vehicle;
                var carType = car.VehicleType;
                if (pattern is ColumnPattern && car.CurrentFuel < carType.TankVolume)
                {
                    if (!_waitingVehicles.Contains(vehicle))
                    {
                        _waitingVehicles.Add(vehicle);
                        OnStartWaiting(vehicle);
                    }

                    double deltaFuel = gameTime.ElapsedGameTime.TotalSeconds * _fillingSpeed;

                    if (car.CurrentFuel + deltaFuel >= carType.TankVolume)
                    {
                        deltaFuel = carType.TankVolume - car.CurrentFuel;

                        if (_waitingVehicles.Contains(vehicle))
                        {
                            _waitingVehicles.Remove(vehicle);
                            OnFinishWaiting(vehicle);
                        }
                    }

                    //filling car
                    Car_Filling(car, carType, ref deltaFuel);

                    //time left to fill car
                    double waitTime = deltaFuel / _fillingSpeed;
                    long waitTimeTicks = (long)(waitTime * TimeSpan.TicksPerSecond);
                    TimeSpan waitTimeSpan = new TimeSpan(waitTimeTicks);
                    gameTime = new GameTime(gameTime.TotalGameTime + waitTimeSpan, gameTime.ElapsedGameTime - waitTimeSpan);

                    result = car.CurrentFuel / carType.TankVolume;
                }
            }

            if (vehicle is TankerVehicle && pattern is TankPattern)
            {
                var tankerType = ((TankerVehicle) vehicle).VehicleType;
                var tankProperty = ((TankPattern) pattern).Property;

                //TODO add events invoking

                if (tankerType.FuelType == tankProperty.Fuel)
                {
                    double deltaFuel = gameTime.ElapsedGameTime.TotalSeconds*_refillingSpeed;

                    //refilling
                    Fuel_Refilling(tankProperty, ref deltaFuel);

                    //time left to refill
                    double waitTime = deltaFuel/_refillingSpeed;
                    long waitTimeTicks = (long) (waitTime*TimeSpan.TicksPerSecond);
                    TimeSpan waitTimeSpan = new TimeSpan(waitTimeTicks);
                    gameTime = new GameTime(gameTime.TotalGameTime + waitTimeSpan, gameTime.ElapsedGameTime - waitTimeSpan);

                    if (tankerType.FuelType == Fuel.A92)
                    {
                        double waitPercent = _stateModel.CurrentFuel92/_stateModel.LimitFuel92;
                        if (waitPercent < 1) _canFill92 = false;
                        if (Math.Abs(waitPercent - 1) < EPS) _canFill92 = true;
                        result = waitPercent;
                    }
                    if (tankerType.FuelType == Fuel.A95)
                    {
                        double waitPercent = _stateModel.CurrentFuel95/_stateModel.LimitFuel95;
                        if (waitPercent < 1) _canFill95 = false;
                        if (Math.Abs(waitPercent - 1) < EPS) _canFill95 = true;
                        result = waitPercent;
                    }
                    if (tankerType.FuelType == Fuel.A98)
                    {
                        double waitPercent = _stateModel.CurrentFuel98/_stateModel.LimitFuel98;
                        if (waitPercent < 1) _canFill98 = false;
                        if (Math.Abs(waitPercent - 1) < EPS) _canFill98 = true;
                        result = waitPercent;
                    }
                    if (tankerType.FuelType == Fuel.Diesel)
                    {
                        double waitPercent = _stateModel.CurrentFuelDiesel/_stateModel.LimitFuelDiesel;
                        if (waitPercent < 1) _canFillDiesel = false;
                        if (Math.Abs(waitPercent - 1) < EPS) _canFillDiesel = true;
                        result = waitPercent;
                    }
                }
            }

            if (vehicle is CashVehicle && pattern == FSModel.GetNearestGameRoadPattern<CashBoxPattern>())
            {
                //TODO add events invoking

                double deltaMoney = gameTime.ElapsedGameTime.TotalSeconds*_cashingSpeed;

                if (_stateModel.CurrentMoney - deltaMoney < 0)
                {
                    deltaMoney = _stateModel.CurrentMoney;
                }

                //cashing
                _stateModel.CurrentMoney -= deltaMoney;

                //time left to cash
                double waitTime = deltaMoney/_cashingSpeed;
                long waitTimeTicks = (long) (waitTime*TimeSpan.TicksPerSecond);
                TimeSpan waitTimeSpan = new TimeSpan(waitTimeTicks);
                gameTime = new GameTime(gameTime.TotalGameTime + waitTimeSpan, gameTime.ElapsedGameTime - waitTimeSpan);

                if (_stateModel.CurrentMoney >= _stateModel.LimitMoney)
                {
                    _canFill92 = false;
                    _canFill95 = false;
                    _canFill98 = false;
                    _canFillDiesel = false;
                    return 1;
                }

                double waitPercent = _stateModel.CurrentMoney/_stateModel.LimitMoney;
                if (waitPercent > 0)
                {
                    _canFill92 = false;
                    _canFill95 = false;
                    _canFill98 = false;
                    _canFillDiesel = false;
                }
                if (Math.Abs(waitPercent) < EPS)
                {
                    _canFill92 = true;
                    _canFill95 = true;
                    _canFill98 = true;
                    _canFillDiesel = true;
                }
                return waitPercent;
            }

            if (Math.Abs(result - 1) < EPS) result = 0;
            return result;
        }

        private void Fuel_Refilling(TankProperty tankProperty, ref double deltaFuel)
        {
            if (tankProperty.Fuel == Fuel.A92)
            {
                if (_stateModel.CurrentFuel92 + deltaFuel > _stateModel.LimitFuel92)
                {
                    deltaFuel = _stateModel.LimitFuel92 - _stateModel.CurrentFuel92;
                }
                _stateModel.CurrentFuel92 += deltaFuel;
            }
            if (tankProperty.Fuel == Fuel.A95)
            {
                if (_stateModel.CurrentFuel95 + deltaFuel > _stateModel.LimitFuel95)
                {
                    deltaFuel = _stateModel.LimitFuel95 - _stateModel.CurrentFuel95;
                }
                _stateModel.CurrentFuel95 += deltaFuel;
            }
            if (tankProperty.Fuel == Fuel.A98)
            {
                if (_stateModel.CurrentFuel98 + deltaFuel > _stateModel.LimitFuel98)
                {
                    deltaFuel = _stateModel.LimitFuel98 - _stateModel.CurrentFuel98;
                }
                _stateModel.CurrentFuel98 += deltaFuel;
            }
            if (tankProperty.Fuel == Fuel.Diesel)
            {
                if (_stateModel.CurrentFuelDiesel + deltaFuel > _stateModel.LimitFuelDiesel)
                {
                    deltaFuel = _stateModel.LimitFuelDiesel - _stateModel.CurrentFuelDiesel;
                }
                _stateModel.CurrentFuelDiesel += deltaFuel;
            }
        }

        private void Car_Filling(CarVehicle car, CarType carType, ref double deltaFuel)
        {
            if (carType.Fuel == Fuel.A92)
            {
                if (_stateModel.CurrentFuel92 < deltaFuel)
                {
                    deltaFuel = _stateModel.CurrentFuel92;
                }
                if (_canFill92)
                {
                    car.CurrentFuel += deltaFuel;
                    _stateModel.CurrentFuel92 -= deltaFuel;
                    _stateModel.CurrentMoney += deltaFuel * Fuel92Price;
                }
            }
            if (carType.Fuel == Fuel.A95)
            {
                if (_stateModel.CurrentFuel95 < deltaFuel)
                {
                    deltaFuel = _stateModel.CurrentFuel95;
                }
                if (_canFill95)
                {
                    car.CurrentFuel += deltaFuel;
                    _stateModel.CurrentFuel95 -= deltaFuel;
                    _stateModel.CurrentMoney += deltaFuel * Fuel95Price;
                }
            }
            if (carType.Fuel == Fuel.A98)
            {
                if (_stateModel.CurrentFuel98 < deltaFuel)
                {
                    deltaFuel = _stateModel.CurrentFuel98;
                }
                if (_canFill98)
                {
                    car.CurrentFuel += deltaFuel;
                    _stateModel.CurrentFuel98 -= deltaFuel;
                    _stateModel.CurrentMoney += deltaFuel * Fuel98Price;
                }
            }
            if (carType.Fuel == Fuel.Diesel)
            {
                if (_stateModel.CurrentFuelDiesel < deltaFuel)
                {
                    deltaFuel = _stateModel.CurrentFuelDiesel;
                }
                if (_canFillDiesel)
                {
                    car.CurrentFuel += deltaFuel;
                    _stateModel.CurrentFuelDiesel -= deltaFuel;
                    _stateModel.CurrentMoney += deltaFuel * FuelDieselPrice;
                }
            }
        }

        public event EventHandler<BaseVehicle> StartWaiting;
        public event EventHandler<BaseVehicle> FinishWaiting;

        protected virtual void OnStartWaiting(BaseVehicle e)
        {
            var handler = StartWaiting;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnFinishWaiting(BaseVehicle e)
        {
            var handler = FinishWaiting;
            if (handler != null) handler(this, e);
        }

        public static double GetWaitingTime(BaseVehicle vehicle)
        {
            double result = 0d;
            if (vehicle is CarVehicle)
            {
                var car = (CarVehicle)vehicle;
                var carType = car.VehicleType;
                //max with zero just in case to avois negative value
                result = Math.Max(0, carType.TankVolume - car.CurrentFuel) / _fillingSpeed;
            }
            return result;
        }
    }
}