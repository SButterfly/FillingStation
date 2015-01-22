using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace SimulationClassLibrary.Kernel
{
    public abstract class GraphicsManager
    {
        #region Intialization

        private readonly PresentationParameters _presentParams;

        public Control ParentControl { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public Size Size { get { return ParentControl.Size; } }
        public Color BackgroundColor { get; set; }
        public float AspectRation { get { return (float)Size.Width / Size.Height; } }

        public ContentManager Content { get; private set; }

        protected GraphicsManager(Control parentControl)
        {
            ParentControl = parentControl;
            _presentParams = new PresentationParameters
            {
                IsFullScreen = false,
                BackBufferWidth = parentControl.ClientSize.Width,
                BackBufferHeight = parentControl.ClientSize.Height,
                DeviceWindowHandle = parentControl.Handle,
                MultiSampleCount = 4
            };

            GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, _presentParams);
            GraphicsDevice.DeviceLost += OnDeviceLost;

            ParentControl.Paint += OnPaint;
            ParentControl.Disposed += OnDisposed;
            ParentControl.SizeChanged += OnSizeChanged;

            BackgroundColor = new Color(0.8f, 0.8f, 0.8f, 0);
            IntializeContentManager();

            Speed = 1d;
        }

        private void IntializeContentManager()
        {
            GameServiceContainer gameProvider = new GameServiceContainer();
            gameProvider.AddService(typeof(IGraphicsDeviceService), new MyGraphicsDeviceService(GraphicsDevice));

            Content = new ContentManager(gameProvider);
        }

        #endregion

        #region Properties

        public TimeSpan PassedTime
        {
            get { return _gameTime == null ? new TimeSpan() : _gameTime.TotalGameTime; }
        }

        public double Speed { get; set; }

        #endregion

        #region Events

        public event EventHandler DrawEvent;
        public event EventHandler UpdateEvent;
        public event EventHandler SizeChanged;

        public event EventHandler Runing;
        public event EventHandler Pausing;
        public event EventHandler Resuming;
        public event EventHandler Exiting;

        #endregion

        #region game control methods

        private Timer _timer;
        private Stopwatch _stopwatch;
        private GameTime _gameTime;
        private readonly EventArgs _emptyEventArgs = EventArgs.Empty;

        public bool IsStarted { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsStoped { get; private set; }

        public void Run()
        {
            Initialize();
            LoadContent();

            IsStarted = true;
            IsRunning = true;
            IsStoped = false;

            _gameTime = new GameTime();
            _lastStopwatchTime = new TimeSpan();
            _stopwatch = new Stopwatch();

            _timer = new Timer();

            _timer.Interval = 1;
            _timer.Tick += TimerTick;

            _timer.Start();
            _stopwatch.Start();

            var starting = Runing;
            if (starting != null) starting(this, _emptyEventArgs);
        }

        public void Pause()
        {
            IsRunning = false;

            _timer.Stop();
            _stopwatch.Stop();

            var pausing = Pausing;
            if (pausing != null) pausing(this, _emptyEventArgs);
        }

        public void Resume()
        {
            IsRunning = true;

            _timer.Start();
            _stopwatch.Start();

            var resuming = Resuming;
            if (resuming != null) resuming(this, _emptyEventArgs);
        }

        public void Exit()
        {
            IsStarted = false;
            IsRunning = false;
            IsStoped = true;

            _timer.Stop();
            _stopwatch.Stop();

            _timer = null;
            _stopwatch = null;

            OnSizeChanged(this, EventArgs.Empty);

            var exiting = Exiting;
            if (exiting != null) exiting(this, _emptyEventArgs);
        }

        #endregion

        #region game methods

        private TimeSpan _lastStopwatchTime;

        private void UpdateGameTime()
        {
            var thisTime = _stopwatch.Elapsed;
            var lastTime = _lastStopwatchTime;

            var deltaGameTime = new TimeSpan((long)(Speed * (thisTime - lastTime).Ticks));

            _gameTime = new GameTime(_gameTime.TotalGameTime + deltaGameTime, deltaGameTime);

            _lastStopwatchTime = thisTime;
        }

        protected virtual void Initialize() { }
        protected virtual void LoadContent() { }
        protected virtual void Update(GameTime gameTime) { }
        protected virtual void Draw(GameTime gameTime) { }
        protected virtual void DrawBackground() { }

        #endregion

        #region private methods

        private void TimerTick(object sender, EventArgs e)
        {
            UpdateGameTime();
            Update(_gameTime);

            var updateEvent = UpdateEvent;
            if (updateEvent != null)
                updateEvent(sender, e);

            ParentControl.Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            GraphicsDevice.Clear(ClearOptions.Target, BackgroundColor, 0.0f, 0);

            DrawBackground();
            if (IsStarted) Draw(_gameTime);
            if (DrawEvent != null)
                DrawEvent(this, _emptyEventArgs);

            GraphicsDevice.Present();
        }

        private void OnDeviceLost(object sender, EventArgs e)
        {
            GraphicsDevice.Reset(_presentParams);
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (ParentControl.ClientSize.Width == 0 || ParentControl.ClientSize.Height == 0) return;

            _presentParams.BackBufferWidth = ParentControl.ClientSize.Width;
            _presentParams.BackBufferHeight = ParentControl.ClientSize.Height;

            GraphicsDevice.Reset(_presentParams);
            ParentControl.Invalidate();

            var sizeChanged = SizeChanged;
            if (sizeChanged != null)
                sizeChanged(sender, e);
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            //hack
            try
            {
                if (GraphicsDevice != null)
                    GraphicsDevice.Dispose();
                GraphicsDevice = null;
            }
            catch (Exception ex) { }
        }

        #endregion
    }
}