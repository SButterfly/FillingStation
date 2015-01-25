using System.Collections.Generic;
using System.Data.OleDb;
using FillingStation.Core.Vehicles;
using FillingStation.DAL.Models;

namespace FillingStation.DAL
{
    public class CarTypeAccessor
    {
        public List<CarType> All()
        {
            try
            {
                List<CarType> allCars = new List<CarType>();

                //Using a string variable to hold the ConnectionString.
                const string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=FillingStation_DB.accdb";

                //Creating an OleDbConnection object, 
                //and then passing in the ConnectionString to the constructor.
                OleDbConnection con = new OleDbConnection(connectionString);

                //Opening the connection.
                con.Open();

                //Use a variable to hold the SQL statement.
                const string selectString = "SELECT car_model_name, image_path, tank_volume, car_type_id, fuel_type_id FROM Car_Model";

                //Creating an OleDbCommand object.
                //Notice that this line passes in the SQL statement and the OleDbConnection object.
                OleDbCommand cmd = new OleDbCommand(selectString, con);

                //Sending the CommandText to the connection, and then building an OleDbDataReader.
                //Note: The OleDbDataReader is forward-only.
                OleDbDataReader reader = cmd.ExecuteReader();

                //Looping through the resultant data selection.
                while (reader.Read())
                {
                    string modelName = reader["car_model_name"].ToString();
                    string imagePath = reader["image_path"].ToString();
                    double tankVolume = double.Parse(reader["tank_volume"].ToString());
                    CarSize carSizeType = (CarSize)int.Parse(reader["car_type_id"].ToString());
                    Fuel fuelType = (Fuel)int.Parse(reader["fuel_type_id"].ToString());
                    CarType car = new CarType(modelName, imagePath, tankVolume, fuelType, carSizeType);
                    allCars.Add(car);
                }

                con.Close();

                return allCars;
            }
            catch
            {
                var allCars = new List<CarType>
                {
                    new CarType("легковая машина 1", "Vehicles/Cars/vh_car_1.png", 60.0, (Fuel) 1, (CarSize) 1),
                    new CarType("легковая машина 2", "Vehicles/Cars/vh_car_2.png", 45.0, (Fuel) 2, (CarSize) 1),
                    new CarType("легковая машина 3", "Vehicles/Cars/vh_car_3.png", 70.0, (Fuel) 3, (CarSize) 1),
                    new CarType("легковая машина 4", "Vehicles/Cars/vh_car_5.png", 65.0, (Fuel) 3, (CarSize) 1),
                    new CarType("грузовая машина 6", "Vehicles/Cars/vh_car_6.png", 150.0, (Fuel) 4, (CarSize) 2),
                    new CarType("легковая машина 7", "Vehicles/Cars/vh_car_7.png", 60.0, (Fuel) 1, (CarSize) 1),
                    new CarType("легковая машина 8", "Vehicles/Cars/vh_car_8.png", 45.0, (Fuel) 2, (CarSize) 1),
                    new CarType("легковая машина 9", "Vehicles/Cars/vh_car_9.png", 70.0, (Fuel) 3, (CarSize) 1),
                    new CarType("легковая машина 10", "Vehicles/Cars/vh_car_10.png", 65.0, (Fuel) 3, (CarSize) 1),
                    new CarType("легковая машина 11", "Vehicles/Cars/vh_car_11.png", 45.0, (Fuel) 2, (CarSize) 1),
                    new CarType("легковая машина 12", "Vehicles/Cars/vh_car_12.png", 70.0, (Fuel) 3, (CarSize) 1),
                    new CarType("грузовая машина 13", "Vehicles/Cars/vh_car_13.png", 170.0, (Fuel) 4, (CarSize) 2),
                    new CarType("легковая машина 14", "Vehicles/Cars/vh_car_14.png", 70.0, (Fuel) 3, (CarSize) 1),
                    new CarType("грузовая машина 15", "Vehicles/Cars/vh_car_15.png", 170.0, (Fuel) 4, (CarSize) 2)
                };

                return allCars;
            }
        }
    }
}