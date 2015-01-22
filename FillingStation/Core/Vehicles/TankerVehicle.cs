using SimulationClassLibrary.Kernel;

namespace FillingStation.Core.Vehicles
{
    public class TankerVehicle : BaseVehicle
    {
        public TankerVehicle(GraphicsManager graphicsManager, TankerType vehicleType)
            : base(graphicsManager, vehicleType)
        {
        }

        public new TankerType VehicleType
        {
            get { return (TankerType)base.VehicleType; }
        }
    }
}