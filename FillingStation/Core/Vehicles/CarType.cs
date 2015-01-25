using FillingStation.DAL.Models;

namespace FillingStation.Core.Vehicles
{
    public enum CarSize { Passenger = 1, Truck = 2 };
    
    public class CarType : BaseVehicleType
    {
        public CarType(string modelName, string imagePath, double tankVolume, Fuel fuel, CarSize carSize)
        {
            ModelName = modelName;
            ImagePath = imagePath;
            TankVolume = tankVolume;
            Fuel = fuel;
            CarSize = carSize;
        }

        public Fuel Fuel { get; private set; }
        public CarSize CarSize { get; private set; }
        public double TankVolume { get; private set; }

        public override string ToString()
        {
            return "CarType {" + "ModelName = " + ModelName + "; ImagePath = " + ImagePath + "; TankVolume = " + TankVolume + "; Fuel = " + Fuel + "; CarSize = " + CarSize + '}';
        } 
    }
}
