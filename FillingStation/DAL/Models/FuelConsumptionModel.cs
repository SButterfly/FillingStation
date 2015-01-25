﻿namespace FillingStation.DAL.Models
{
    public enum Fuel { A92 = 1, A95 = 2, A98 = 3, Diesel = 4 };

    public class FuelConsumptionModel
    {
        public FuelConsumptionModel()
        { }

        public FuelConsumptionModel(Fuel fuel, int carPercentage, double fillingVolume, double price)
        {
            Fuel = fuel;
            CarPercentage = carPercentage;
            FillingVolume = fillingVolume;
            Price = price;
        }

        public Fuel Fuel { get; set; }
        public int CarPercentage { get; set; }
        public double FillingVolume { get; set; }
        public double Price { get; set; }
    }
}