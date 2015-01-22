using SimulationClassLibrary.Kernel;

namespace FillingStation.Core.Vehicles
{
    public class CashVehicle : BaseVehicle
    {
        public CashVehicle(GraphicsManager graphicsManager, CasherType vehicleType)
            : base(graphicsManager, vehicleType)
        {
        }

        public new CasherType VehicleType
        {
            get { return (CasherType)base.VehicleType; }
        }
    }
}