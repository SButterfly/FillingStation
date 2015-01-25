using FillingStation.DAL.Models;
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
            ImagePath = Strings.Tanker_path;
            ModelName = Strings.Tanker;
        }

        public Fuel FuelType { get; private set; }
    }
}
