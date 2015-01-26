using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Core.Vehicles;
using FillingStation.DAL.Models;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Extensions
{
    public static class FuelExtensions
    {
        public static Fuel ToFuel(this string fuel)
        {
            if (fuel == Strings.Fuel_a92)
                return Fuel.A92;
            if (fuel == Strings.Fuel_a95)
                return Fuel.A95;
            if (fuel == Strings.Fuel_a98)
                return Fuel.A98;
            if (fuel == Strings.Fuel_diesel)
                return Fuel.Diesel;

            throw new ArgumentException(Strings.Exception_fuel);
        }

        public static string ToString(this Fuel fuel)
        {
            switch (fuel)
            {
                case Fuel.A92:
                    return Strings.Fuel_a92;
                case Fuel.A95:
                    return Strings.Fuel_a95;
                case Fuel.A98:
                    return Strings.Fuel_a98;
                case Fuel.Diesel:
                    return Strings.Fuel_diesel;
                default:
                    Logger.WriteLine(typeof(FuelExtensions), "There are no suitable string.");
                    return fuel.ToString();
            }
        }
    }
}