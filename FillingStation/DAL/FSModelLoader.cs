using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using FillingStation.Core.Models;
using FillingStation.Core.Patterns;
using FillingStation.Core.Properties;
using FillingStation.Core.Vehicles;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.DAL
{
    public class FSModelLoader
    {
        public void Save(FSModel model, string path)
        {
            var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new BinaryWriter(fileStream);

            writer.Write(model.Width);
            writer.Write(model.Height);
            writer.Write(model.Patterns.Count());
            foreach (var pattern in model.Patterns)
            {
                writer.Write(ToInt(pattern));

                var point = model.Get(pattern);
                writer.Write(point.X);
                writer.Write(point.Y);

                if (pattern.Property is ITurnProperty)
                {
                    writer.Write((int)(pattern.Property as ITurnProperty).Angle);
                }
                if (pattern is CashBoxPattern)
                {
                    writer.Write((pattern.Property as CashBoxProperty).CashBoxLimit);
                }
                if (pattern is TankPattern)
                {
                    writer.Write((int)(pattern.Property as TankProperty).Fuel);
                    writer.Write((pattern.Property as TankProperty).TankLimit);
                    writer.Write((pattern.Property as TankProperty).CriticalTankLimit);
                    writer.Write((pattern.Property as TankProperty).LowTankLimit);
                }
            }
            writer.Close();
        }
        public FSModel Read(string path)
        {
            try
            {
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var reader = new BinaryReader(fileStream);

                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                var count = reader.ReadInt32();

                var model = new FSModel(width, height);
                for (int i = 0; i < count; i++)
                {
                    var pattern = ToPattern(reader.ReadInt32());
                    var point = new Point(reader.ReadInt32(), reader.ReadInt32());

                    model.Add(pattern, point);

                    if (pattern.Property is ITurnProperty)
                    {
                        (pattern.Property as ITurnProperty).Angle = (Rotation)reader.ReadInt32();
                    }

                    if (pattern is CashBoxPattern)
                    {
                        (pattern.Property as CashBoxProperty).CashBoxLimit = reader.ReadInt32();
                    }
                    if (pattern is TankPattern)
                    {
                        (pattern.Property as TankProperty).Fuel = (Fuel)reader.ReadInt32();
                        (pattern.Property as TankProperty).TankLimit = reader.ReadInt32();
                        (pattern.Property as TankProperty).CriticalTankLimit = reader.ReadInt32();
                        (pattern.Property as TankProperty).LowTankLimit = reader.ReadInt32();
                    }
                }
                return model;
            }
            catch (Exception e)
            {
                throw new Exception(Strings.Exception_readFS + e.Message, e);
            }
        }

        private static readonly DoubleDictionary<Type, int> _dictionary = new DoubleDictionary<Type, int>
            {
                {typeof (CashBoxPattern), 0},
                {typeof (EnterPattern), 1},
                {typeof (ExitPattern), 2},
                {typeof (InfoTablePattern), 3},
                {typeof (ColumnPattern), 4},
                {typeof (MainRoadPattern), 5},
                {typeof (TankPattern), 6},
                {typeof (RoadInPattern), 7},
                {typeof (RoadOutPattern), 8},
                {typeof (RoadTPattern), 9},
                {typeof (RoadPattern), 10},
                {typeof (RoadTurnPattern), 11}
            };

        private static int ToInt(IPattern pattern)
        {
            return _dictionary[pattern.GetType()];
        }

        private static IPattern ToPattern(int value)
        {
            return (IPattern) Activator.CreateInstance(_dictionary.GetKey(value));
        }
    }
}
