using System;
using System.Collections.Generic;
using FillingStation.Core.Vehicles;
using FillingStation.DAL.Models;
using FillingStation.Properties;

namespace FillingStation.DAL
{
    public class FuelModelAccessor
    {
        private const string keyFormat = "{0}{1}";
        private const string percentageKey = "percentage";
        private const string volumeKey = "volume";
        private const string priceKey = "price";

        public IList<FuelConsumptionModel> All()
        {
            var list = new List<FuelConsumptionModel>();

            foreach (Fuel value in Enum.GetValues(typeof(Fuel)))
            {
                var carPercentageKey = string.Format(keyFormat, value, percentageKey);
                var carVolumeKey = string.Format(keyFormat, value, volumeKey);
                var carPriceKey = string.Format(keyFormat, value, priceKey);

                FuelConsumptionModel model;

                try
                {
                    int percentage = (int)Settings.Default[carPercentageKey];
                    double volume = (double)Settings.Default[carVolumeKey];
                    double price = (double)Settings.Default[carPriceKey];

                    model = new FuelConsumptionModel(value, percentage, volume, price);
                }
                catch
                {
                    const int percentage = 25;
                    const double volume = 40;
                    const double price = 36;

                    model = new FuelConsumptionModel(value, percentage, volume, price);
                    Save(model);
                }

                list.Add(model);
            }

            return list;
        }

        public void Save(FuelConsumptionModel model)
        {
            var carPercentageKey = string.Format(keyFormat, model.Fuel, percentageKey);
            var carVolumeKey = string.Format(keyFormat, model.Fuel, volumeKey);
            var carPriceKey = string.Format(keyFormat, model.Fuel, priceKey);

            Settings.Default[carPercentageKey] = model.CarPercentage;
            Settings.Default[carVolumeKey] = model.FillingVolume;
            Settings.Default[carPriceKey] = model.Price;

            Settings.Default.Save();
        }

        public void Save(IEnumerable<FuelConsumptionModel> list)
        {
            foreach (var model in list)
            {
                Save(model);
            }
        }
    }
}
