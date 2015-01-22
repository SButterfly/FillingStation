using System.Collections.Generic;
using System.Data.OleDb;
using FillingStation.Core.Vehicles;

namespace FillingStation.DAL
{
    class FuelTypeAccessor
    {
        public List<FuelType> All()
        {
            try
            {
                List<FuelType> allFuelTypes = new List<FuelType>();

                //Using a string variable to hold the ConnectionString.
                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=FillingStation_DB.accdb";

                //Creating an OleDbConnection object, 
                //and then passing in the ConnectionString to the constructor.
                OleDbConnection con = new OleDbConnection(connectionString);

                //Opening the connection.
                con.Open();

                //Use a variable to hold the SQL statement.
                string selectString = "SELECT fuel_type_id, fuel_type_name, price FROM Fuel_Type";

                //Creating an OleDbCommand object.
                //Notice that this line passes in the SQL statement and the OleDbConnection object.
                OleDbCommand cmd = new OleDbCommand(selectString, con);

                //Sending the CommandText to the connection, and then building an OleDbDataReader.
                //Note: The OleDbDataReader is forward-only.
                OleDbDataReader reader = cmd.ExecuteReader();

                //Looping through the resultant data selection.
                while (reader.Read())
                {
                    string fuelTypeName = reader["fuel_type_name"].ToString();
                    double pricePerLiter = double.Parse(reader["price"].ToString());
                    Fuel fuelTypeID = (Fuel)int.Parse(reader["fuel_type_id"].ToString());
                    FuelType fuel = new FuelType(fuelTypeID, pricePerLiter);
                    allFuelTypes.Add(fuel);
                }

                con.Close();

                return allFuelTypes;
            }
            catch
            {
                List<FuelType> allFuelTypes = new List<FuelType>();

                allFuelTypes.Add(new FuelType((Fuel)1, 33.55));
                allFuelTypes.Add(new FuelType((Fuel)2, 36.45));
                allFuelTypes.Add(new FuelType((Fuel)3, 40.55));
                allFuelTypes.Add(new FuelType((Fuel)4, 34.40));

                return allFuelTypes;
            }
        }
    }
}
