using System.Collections.Generic;
using FillingStation.DAL.Models;

namespace FillingStation.DAL
{
    public class FuelModelAccessor
    {
        public IList<FuelConsumptionModel> All()
        {
            var list = new List<FuelConsumptionModel>
            {
                new FuelConsumptionModel()
                {
                    CarPercentage = 25,
                    FillingVolume = 40,
                    Fuel = Fuel.A92,
                    Price = 33
                },
                new FuelConsumptionModel()
                {
                    CarPercentage = 25,
                    FillingVolume = 40,
                    Fuel = Fuel.A95,
                    Price = 36
                },
                new FuelConsumptionModel()
                {
                    CarPercentage = 25,
                    FillingVolume = 40,
                    Fuel = Fuel.A98,
                    Price = 39
                },
                new FuelConsumptionModel()
                {
                    CarPercentage = 25,
                    FillingVolume = 70,
                    Fuel = Fuel.Diesel,
                    Price = 35
                }
            };

            return list;
        }

        public void Save(FuelConsumptionModel model)
        {
            //TODO implement
        }

        public void SaveAll(IEnumerable<FuelConsumptionModel> list)
        {
            foreach (var model in list)
            {
                Save(model);
            }
        }
    }
}
