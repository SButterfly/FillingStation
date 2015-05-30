using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Core.Graph;
using FillingStation.Core.Patterns;
using FillingStation.Core.SimulationServices;
using FillingStation.Core.Vehicles;
using FillingStation.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SimulationClassLibrary.Kernel;
using Point = System.Drawing.Point;

namespace FillingStation.Core.Models
{
    /// <summary>
    /// Sync class between FSModel and simulation classes
    /// </summary>
    public class FieldModel : XnaObject
    {
        #region Fields

        private readonly LinkedList<BaseVehicle> _queueVehicles = new LinkedList<BaseVehicle>();
        private readonly TimeSpan _maxUpdateTimeSecond = TimeSpan.FromSeconds(0.3);

        #endregion

        #region Intialization

        public FieldModel(float cellWidth, float cellHeight, GraphicsManager graphicsManager, 
            FSModel model, VehicleStream vehicleStream, VehicleAwaiter vehicleAwaiter)
            : base(graphicsManager, null)
        {
            CellWidth = cellWidth;
            CellHeight = cellHeight;

            Model = model;
            VehicleStream = vehicleStream;
            VehicleAwaiter = vehicleAwaiter;
            FieldGraph = CreateFieldGraph(model);

            PatternChooser = new PatternChooser(model, this, FieldGraph);

            VehicleMover = new VehicleMover(FieldGraph, vehicleAwaiter, PatternChooser);
        }

        private FieldGraph CreateFieldGraph(FSModel model)
        {
            var graph = new FieldGraph();

            var startPattern1 = new EnterPattern();
            var startPoint = new Point(model.Width, model.Height);
            graph.Add(startPattern1, startPoint);

            var startPattern2 = new EnterPattern();
            var startPoint2 = new Point(model.Width, model.Height+1);
            graph.Add(startPattern2, startPoint2);

            IGameRoadPattern lastPattern1 = startPattern1;
            IGameRoadPattern lastPattern2 = startPattern2;

            for (int i = model.Width - 1; i >= 0; i--)
            {
                var pattern1 = new MainRoadPattern();
                var vector1 = new Point(i, model.Height);
                graph.Add(lastPattern1, pattern1, vector1);

                var pattern2 = new MainRoadPattern();
                var vector2 = new Point(i, model.Height + 1);
                graph.Add(lastPattern2, pattern2, vector2);

                lastPattern1 = pattern1;
                lastPattern2 = pattern2;
            }

            var endPattern1 = new ExitPattern();
            var endVector1 = new Point(-1, model.Height);
            graph.Add(lastPattern1, endPattern1, endVector1);

            var endPattern2 = new ExitPattern();
            var endVector2 = new Point(-1, model.Height + 1);
            graph.Add(lastPattern2, endPattern2, endVector2);

            graph.Merge(model.GenerateGraph());
            return graph;
        }

        #endregion

        #region Properties

        public float CellWidth { get; private set; }
        public float CellHeight { get; private set; }

        public FSModel Model { get; private set; }
        public VehicleStream VehicleStream { get; private set; }
        public VehicleAwaiter VehicleAwaiter { get; private set; }
        public PatternChooser PatternChooser { get; set; }

        public FieldGraph FieldGraph { get; private set; }
        public VehicleMover VehicleMover { get; private set; }

        public IEnumerable<BaseVehicle> VehiclesInQueue { get { return _queueVehicles; } }

        #endregion

        #region Game methods

        public override void Update(GameTime gameTime)
        {
            var totalTime = gameTime.TotalGameTime;
            var updateTime = gameTime.ElapsedGameTime;

            while (updateTime > _maxUpdateTimeSecond)
            {
                totalTime += _maxUpdateTimeSecond;
                updateTime -= _maxUpdateTimeSecond;
                UpdateVehicles(new GameTime(totalTime, _maxUpdateTimeSecond));
            }

            UpdateVehicles(new GameTime(totalTime, updateTime));

            if (_queueVehicles.Count > 5)
            {
                Logger.WriteLine(this, "Vehicles at queue more than 5");
            }
        }

        private void UpdateVehicles(GameTime gameTime)
        {
            VehicleMover.RemoveLeftVehicles();
            VehicleMover.UpdateVehicles(gameTime);

            var newCars = VehicleStream.GetNewCars(gameTime, VehicleMover.Vehicles.Concat(_queueVehicles));
            if (newCars != null && newCars.Count > 0)
            {
                foreach (var carType in newCars)
                {
                    var vehicle = VehicleFactory.CreateVehicle(this, carType);
                    vehicle.Scale = Math.Min((CellWidth - 5f) / vehicle.Size.Width, 1f);

                    if (carType is CarType)
                    {
                        _queueVehicles.AddLast(vehicle);
                    }
                    else
                    {
                        Logger.WriteLine(this, "Get service vehicle: " + carType);
                        _queueVehicles.AddFirst(vehicle);
                    }
                }
            }

            while (_queueVehicles.Count > 0)
            {
                var vehicle = _queueVehicles.First.Value;
                var pattern = PatternChooser.ChooseStartPattern(vehicle);

                if (VehicleMover.IsPatternFree(pattern))
                {
                    _queueVehicles.RemoveFirst();
                    VehicleMover.AddVehicle(vehicle, pattern);
                    VehicleMover.UpdateVehicle(vehicle, gameTime);
                }
                else
                {
                    break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (var vehicle in VehicleMover.Vehicles)
            {
                float x = vehicle.Position.X*CellWidth;
                float y = vehicle.Position.Y*CellHeight;

                vehicle.Draw(spriteBatch, new Vector2(x, y), position);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 offset)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        protected override Texture2D LoadTexture2D(string fileName)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}