using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using FillingStation.Annotations;
using FillingStation.Core.Generators;
using FillingStation.Core.Models;
using FillingStation.Core.SimulationServices;
using FillingStation.Core.Vehicles;
using FillingStation.DAL;
using FillingStation.Helpers;
using FillingStation.Localization;
using FillingStation.Properties;
using SimulationClassLibrary;

namespace FillingStation.Views
{
    public partial class SimulationView : Window, INotifyPropertyChanged
    {
        private readonly Visualization _game;
        private readonly FSModel _model;

        public SimulationView(FSModel model, Stream fieldStream)
        {
            InitializeComponent();
            menu.Items.Remove(speedTextBox);
            DataContext = this;

            _model = model;

            _game = new Visualization(xnaRenderPanel, "Assets");
            _game.LoadBackgroundField(fieldStream);
            _game.UpdateEvent += (sender, args) => OnPropertyChanged("AdditionalText");

            stateControl.FSStateModel = new FSStateModel(_model);

            fieldStream.Close();

            Closed += (sender, args) =>
            {
                if (StopSimulationEnabled)
                {
                    StopSimulation.Execute();
                }
                FSVehicleFactory.ClearResources();
            };
        }

        #region Properties

        public Command StartSimulation
        {
            get
            {
                return new Command(() =>
                {
                    try
                    {
                        _game.Speed = GetComboboxValue();
                        if (_game.IsStarted)
                        {
                            _game.Resume();
                        }
                        else
                        {
                            var model = CreateModel();
                            statisticsControl.FSStatisticsModel = new FSStatisticsModel(model.VehicleMover, () => _game.PassedTime);
                            _game.SimulationModel = model;
                            _game.Run();
                            generatorControl.IsEnabled = false;
                        }
                        OnPropertyChanged("StartSimulationEnabled");
                        OnPropertyChanged("PauseSimulationEnabled");
                        OnPropertyChanged("StopSimulationEnabled");
                    }
                    catch (Exception e)
                    {
                        MessageDialog.ShowException(e);
                    }
                });
            }
        }

        public Command PauseSimulation
        {
            get
            {
                return new Command(() =>
                {
                    _game.Pause();
                    OnPropertyChanged("StartSimulationEnabled");
                    OnPropertyChanged("PauseSimulationEnabled");
                    OnPropertyChanged("StopSimulationEnabled");
                });
            }
        }

        public Command StopSimulation
        {
            get
            {
                return new Command(() =>
                {
                    _game.Exit();
                    statisticsControl.FSStatisticsModel = FSStatisticsModel.Empty;
                    stateControl.FSStateModel = new FSStateModel(_model);
                    generatorControl.IsEnabled = true;
                    OnPropertyChanged("StartSimulationEnabled");
                    OnPropertyChanged("PauseSimulationEnabled");
                    OnPropertyChanged("StopSimulationEnabled");
                });
            }
        }

        public bool StartSimulationEnabled
        {
            get { return !_game.IsRunning; }
        }
        public bool PauseSimulationEnabled
        {
            get { return _game.IsRunning; }
        }
        public bool StopSimulationEnabled
        {
            get { return _game.IsStarted; }
        }

        public string AdditionalText
        {
            get { return Strings.ModelTime + _game.PassedTime.ToString("g"); }
        }

        #endregion

        #region Methods

        private FieldModel CreateModel()
        {
            IGenerator generator = generatorControl.Generator;
            VehicleStream vehicleStream = new VehicleStream(generator, stateControl.FSStateModel);
            VehicleAwaiter vehicleAwaiter = new VehicleAwaiter(_model, stateControl.FSStateModel);

            FSVehicleFactory.Init(_game, new CarTypeAccessor().All().Select(carType => carType.ImagePath));

            var cellWidth = Settings.Default.cellWidth;
            var cellHeight = Settings.Default.cellHeight;

            var model = new FieldModel(cellWidth, cellHeight, _game, _model, vehicleStream, vehicleAwaiter);
            return model;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_game == null) return;

            menu.Items.Remove(speedTextBox);
            speedTextBox.Visibility = Visibility.Collapsed;

            if (comboBox.SelectedIndex == 0)
            {
                _game.Speed = 0.5;
            }
            if (comboBox.SelectedIndex == 1)
            {
                _game.Speed = 1;
            }
            if (comboBox.SelectedIndex == 2)
            {
                _game.Speed = 2;
            }
            if (comboBox.SelectedIndex == 3)
            {
                _game.Speed = 4;
            }
            if (comboBox.SelectedIndex == 4)
            {
                _game.Speed = 10;
            }
            if (comboBox.SelectedIndex == 5)
            {
                _game.Speed = 20;
            }
            if (comboBox.SelectedIndex == 6)
            {
                _game.Speed = 60;
            }
            if (comboBox.SelectedIndex == 7)
            {
                menu.Items.Add(speedTextBox);
                speedTextBox.Visibility = Visibility.Visible;
            }
        }

        private double GetComboboxValue()
        {
            var value = 1d;
            if (comboBox.SelectedIndex == 0)
            {
                value = 0.5;
            }
            if (comboBox.SelectedIndex == 1)
            {
                value = 1;
            }
            if (comboBox.SelectedIndex == 2)
            {
                value = 2;
            }
            if (comboBox.SelectedIndex == 3)
            {
                value = 4;
            }
            if (comboBox.SelectedIndex == 4)
            {
                value = 10;
            }
            if (comboBox.SelectedIndex == 5)
            {
                value = 20;
            }
            if (comboBox.SelectedIndex == 6)
            {
                value = 60;
            }
            if (comboBox.SelectedIndex == 7)
            {
                try
                {
                    value = double.Parse(speedTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    throw new Exception(Strings.Exception_SimulationSpeed + e.Message);
                }
                if (!(0 < value && value <= 60)){
                    throw new Exception(string.Format(Strings.Exception_SimulationSpeedRange, 0, 60));
                }
            }
            return value;
        }

        #endregion

        #region Notification region

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
