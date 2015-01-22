using System;
using Microsoft.Xna.Framework.Graphics;

namespace SimulationClassLibrary.Kernel
{
    public class MyGraphicsDeviceService : IGraphicsDeviceService
    {
        public MyGraphicsDeviceService(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public GraphicsDevice GraphicsDevice { get; private set; }

        public event EventHandler<EventArgs> DeviceDisposing
        {
            add { GraphicsDevice.Disposing += value; }
            remove { GraphicsDevice.Disposing -= value; }
        }

        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceReset
        {
            add { GraphicsDevice.DeviceReset += value; }
            remove { GraphicsDevice.DeviceReset -= value; }
        }

        public event EventHandler<EventArgs> DeviceResetting
        {
            add { GraphicsDevice.DeviceResetting -= value; }
            remove { GraphicsDevice.DeviceResetting -= value; }
        }
    }
}