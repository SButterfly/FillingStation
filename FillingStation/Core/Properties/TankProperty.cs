using System;
using System.Collections.Generic;
using FillingStation.Core.Vehicles;
using FillingStation.DAL.Models;
using FillingStation.Extensions;
using FillingStation.Helpers;
using FillingStation.Localization;
using Newtonsoft.Json;

namespace FillingStation.Core.Properties
{
    public class TankProperty : BaseProperty
    {
        public override string PatternName
        {
            get { return Strings.Tank; }
        }

        private int _tankLimit = 10000;
        [JsonProperty("tank")]
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
        [JsonProperty("critical")]
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
        [JsonProperty("low")]
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
        [JsonProperty("fuel")]
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

        [JsonIgnore]
        public string FuelName
        {
            get { return Strings.Tank_fuel_field; }
        }

        [JsonIgnore]
        public string TankLimitName
        {
            get { return Strings.Tank_limit; }
        }

        [JsonIgnore]
        public string CriticalTankLimitName
        {
            get { return Strings.Tank_fuel_critical; }
        }

        [JsonIgnore]
        public string LowTankLimitName
        {
            get { return Strings.Tank_fuel_low; }
        }

        [JsonIgnore]
        public EnumService<Fuel> FuelService { get { return new FuelEnumService(); } }

        public override void Clone(IProperty property)
        {
            base.Clone(property);
            var tankProperty = property as TankProperty;
            if (tankProperty != null)
            {
                TankLimit = tankProperty.TankLimit;
                LowTankLimit = tankProperty.LowTankLimit;
                CriticalTankLimit = tankProperty.CriticalTankLimit;
                Fuel = tankProperty.Fuel;
            }
        }

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