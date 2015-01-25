using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FillingStation.Core.Vehicles;

namespace FillingStation.DAL.Models
{
    class FuelConsumptionModel
    {
        public Fuel fuelType { get; set; }
        public int carPercentage { get; set; }
        public double fillingVolume { get; set; }
        public double price { get; set; }
    }
}
