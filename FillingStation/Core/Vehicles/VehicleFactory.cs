using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FillingStation.Core.Models;
using FillingStation.Localization;
using Microsoft.Xna.Framework.Graphics;
using SimulationClassLibrary.Kernel;

namespace FillingStation.Core.Vehicles
{
    public static class FSVehicleFactory
    {
        private static readonly Dictionary<string, Texture2D> _dictionary = new Dictionary<string, Texture2D>();

        public static void Init(GraphicsManager graphicsManager, IEnumerable<string> imagePathes)
        {
            foreach (var str in imagePathes.Concat(new string[] { "Vehicles/vh_tanker.png", "Vehicles/vh_casher.png" }))
            {
                if (!_dictionary.ContainsKey(str))
                {
                    var texture = LoadTexture2D(str, graphicsManager);
                    _dictionary.Add(str, texture);
                }
            }
        }

        public static BaseVehicle CreateVehicle(FieldModel simulationStrategy, BaseVehicleType vehicleType)
        {
            BaseVehicle vehicle = null;
            if (vehicleType is CarType)
                vehicle = new CarVehicle(simulationStrategy.GraphicsManager, (CarType) vehicleType);

            if (vehicleType is CasherType)
                vehicle = new CashVehicle(simulationStrategy.GraphicsManager, (CasherType) vehicleType);

            if (vehicleType is TankerType)
                vehicle = new TankerVehicle(simulationStrategy.GraphicsManager, (TankerType) vehicleType);

            if (vehicle == null)
                throw new TypeUnloadedException(Strings.Exception_missingClass);

            vehicle.LoadContent(_dictionary[vehicleType.ImagePath]);
            return vehicle;
        }

        public static void ClearResources()
        {
            _dictionary.Clear();
        }

        private static Texture2D LoadTexture2D(string fileName, GraphicsManager graphicsManager)
        {
            using (var fileStream = new FileStream(graphicsManager.Content.RootDirectory +"/"+ fileName, FileMode.Open))
            {
                return Texture2D.FromStream(graphicsManager.GraphicsDevice, fileStream);
            }
        }
    }
}