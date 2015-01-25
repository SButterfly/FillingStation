using System.Collections.Generic;
using FillingStation.Core.Vehicles;
using FillingStation.DAL.Models;

namespace FillingStation.DAL
{
    public class CarTypeAccessor
    {
        public List<CarType> All()
        {
            var fuelData = new FuelModelAccessor().All();
            var allCars = new List<CarType>(12);

            foreach (var model in fuelData)
            {
                if (model.Fuel == Fuel.A92)
                {
                    allCars.Add(new CarType("car a92 1", "Vehicles/Cars/vh_car_1.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                    allCars.Add(new CarType("car a92 2", "Vehicles/Cars/vh_car_2.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                    allCars.Add(new CarType("car a92 3", "Vehicles/Cars/vh_car_3.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                }
                if (model.Fuel == Fuel.A95)
                {
                    allCars.Add(new CarType("car a95 1", "Vehicles/Cars/vh_car_4.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                    allCars.Add(new CarType("car a95 2", "Vehicles/Cars/vh_car_5.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                    allCars.Add(new CarType("car a95 3", "Vehicles/Cars/vh_car_6.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                }
                if (model.Fuel == Fuel.A98)
                {
                    allCars.Add(new CarType("car a98 1", "Vehicles/Cars/vh_car_7.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                    allCars.Add(new CarType("car a98 2", "Vehicles/Cars/vh_car_8.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                    allCars.Add(new CarType("car a98 3", "Vehicles/Cars/vh_car_9.png", model.FillingVolume, model.Fuel, CarSize.Passenger));
                }
                if (model.Fuel == Fuel.Diesel)
                {
                    allCars.Add(new CarType("car diesel 1", "Vehicles/Cars/vh_car_10.png", model.FillingVolume, model.Fuel, CarSize.Truck));
                    allCars.Add(new CarType("car diesel 2", "Vehicles/Cars/vh_car_11.png", model.FillingVolume, model.Fuel, CarSize.Truck));
                    allCars.Add(new CarType("car diesel 3", "Vehicles/Cars/vh_car_12.png", model.FillingVolume, model.Fuel, CarSize.Truck));
                }
            }

            return allCars;
        }
    }
}