using System;
using System.Collections.Generic;
using FillingStation.Core.Vehicles;
using FillingStation.Extensions;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Core.Properties
{
    public class TankProperty : BaseProperty
    {
        public override string PatternName
        {
            get { return Strings.Tank; }
        }

        private int _tankLimit = 10000;
        public int TankLimit
        {
            get { return _tankLimit; }
            set
            {
                if (value == _tankLimit) return;
                _tankLimit = value;
                OnPropertyChanged();
            }
        }

        private int _criticalTankLimit = 2000;
        public int CriticalTankLimit
        {
            get { return _criticalTankLimit; }
            set
            {
                if (value == _criticalTankLimit) return;
                _criticalTankLimit = value;
                OnPropertyChanged();
            }
        }
        private int _lowTankLimit = 3000;
        public int LowTankLimit
        {
            get { return _lowTankLimit; }
            set
            {
                if (value == _lowTankLimit) return;
                _lowTankLimit = value;
                OnPropertyChanged();
            }
        }

        private Fuel _fuel = Fuel.A92;
        public Fuel Fuel
        {
            get { return _fuel; }
            set
            {
                if (value == _fuel) return;
                _fuel = value;
                OnPropertyChanged();
            }
        }

        public string FuelName
        {
            get { return Strings.Tank_fuel_field; }
        }
        public string TankLimitName
        {
            get { return Strings.Tank_limit; }
        }
        public string CriticalTankLimitName
        {
            get { return Strings.Tank_fuel_critical; }
        }
        public string LowTankLimitName
        {
            get { return Strings.Tank_fuel_low; }
        }

        public EnumService<Fuel> FuelService { get { return new FuelEnumService(); } }

        private class FuelEnumService : EnumService<Fuel>
        {
            public override IEnumerable<Fuel> AllItems()
            {
                return ((Fuel[])Enum.GetValues(typeof(Fuel)));
            }
            public override Fuel ToEnum(object value)
            {
                return FuelExtensions.ToFuel(value as string);
            }
            public override object ToObject(Fuel value)
            {
                return FuelExtensions.ToString(value);
            }
        }
    }
}