using FillingStation.Localization;

namespace FillingStation.Core.Vehicles
{
    public class CasherType : BaseVehicleType
    {
        public CasherType()
        {
            ImagePath = Strings.Casher_path;
            ModelName = Strings.Casher;
        }
    }
}
