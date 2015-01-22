using SimulationClassLibrary.Kernel;

namespace FillingStation.Core.Vehicles
{
    public class CarVehicle : BaseVehicle
    {
        public CarVehicle(GraphicsManager graphicsManager, CarType vehicleType)
            : base(graphicsManager, vehicleType)
        {
        }

        public new CarType VehicleType
        {
            get { return (CarType)base.VehicleType; }
        }

        public double CurrentFuel { get; set; }
    }
}