using System;
using FillingStation.Localization;

namespace FillingStation.Core.Vehicles
{
    public class FuelType
    {
        public FuelType(Fuel fuel, double pricePerLiter)
        {
            Fuel = fuel;
            PricePerLiter = pricePerLiter;
        }

        public Fuel Fuel { get; private set; }
        public double PricePerLiter { get; private set; }

        public override string ToString()
        {
            return "Fuel {" + "Fuel = " + Fuel + "; PricePerLiter = " + PricePerLiter + '}';
        }
    }
    public class TankerType : BaseVehicleType
    {
        public TankerType(Fuel fuelType)
        {
            FuelType = fuelType;
            ImagePath = GetTankerPath(fuelType);
            ModelName = Strings.Tanker;
        }

        public Fuel FuelType { get; private set; }

        private static string GetTankerPath(Fuel fuel)
        {
            if (fuel == Fuel.A92)
            {
                return "Vehicles/vh_tanker_1.png";
            }
            if (fuel == Fuel.A95)
            {
                return "Vehicles/vh_tanker_2.png";
            }
            if (fuel == Fuel.A98)
            {
                return "Vehicles/vh_tanker_3.png";
            }
            if (fuel == Fuel.Diesel)
            {
                return "Vehicles/vh_tanker_4.png";
            }

            throw new ArgumentException();
        }
    }
}
