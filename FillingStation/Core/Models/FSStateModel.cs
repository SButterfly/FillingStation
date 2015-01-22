using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FillingStation.Annotations;
using FillingStation.Core.Patterns;
using FillingStation.Core.Properties;
using FillingStation.Core.Vehicles;

namespace FillingStation.Core.Models
{
    public class FSStateModel : INotifyPropertyChanged
    {
        private FSStateModel()
        {
            LimitMoney = new CashBoxProperty().CashBoxLimit;
            CurrentFuel92 = new TankProperty().TankLimit;
            CurrentFuel95 = CurrentFuel92;
            CurrentFuel98 = CurrentFuel92;
            CurrentFuelDiesel = CurrentFuel92;
        }

        public FSStateModel(FSModel model)
        {
            FSModel = model;
            Clear();
        }

        public void Clear()
        {
            CurrentMoney = 0;

            var cashBox = FSModel.Patterns.OfType<CashBoxPattern>().First();
            LimitMoney = cashBox.Property.CashBoxLimit;

            var tankProperties = FSModel.Patterns.OfType<TankPattern>().Select(pattern => pattern.Property);
            foreach (var tankProperty in tankProperties)
            {
                if (tankProperty.Fuel == Fuel.A92)
                {
                    CurrentFuel92 = tankProperty.TankLimit;
                    LimitFuel92 = tankProperty.TankLimit;
                    LowFuel92 = tankProperty.LowTankLimit;
                    CriticalFuel92 = tankProperty.CriticalTankLimit;
                }
                if (tankProperty.Fuel == Fuel.A95)
                {
                    CurrentFuel95 = tankProperty.TankLimit;
                    LimitFuel95 = tankProperty.TankLimit;
                    LowFuel95 = tankProperty.LowTankLimit;
                    CriticalFuel95 = tankProperty.CriticalTankLimit;
                }
                if (tankProperty.Fuel == Fuel.A98)
                {
                    CurrentFuel98 = tankProperty.TankLimit;
                    LimitFuel98 = tankProperty.TankLimit;
                    LowFuel98 = tankProperty.LowTankLimit;
                    CriticalFuel98 = tankProperty.CriticalTankLimit;
                }
                if (tankProperty.Fuel == Fuel.Diesel)
                {
                    CurrentFuelDiesel = tankProperty.TankLimit;
                    LimitFuelDiesel = tankProperty.TankLimit;
                    LowFuelDiesel = tankProperty.LowTankLimit;
                    CriticalFuelDiesel = tankProperty.CriticalTankLimit;
                }
            }
        }

        public static FSStateModel Empty
        {
            get { return new FSStateModel(); }
        }

        #region Properties

        private double _currentMoney;
        public double CurrentMoney
        {
            get { return _currentMoney; }
            set
            {
                if (_currentMoney == value) return;
                _currentMoney = value;
                OnPropertyChanged();
            }
        }

        private double _currentFuel92;
        public double CurrentFuel92 
        {
            get { return _currentFuel92; }
            set
            {
                if (_currentFuel92 == value) return;
                _currentFuel92 = value;
                OnPropertyChanged();
            }
        }

        private double _currentFuel95;
        public double CurrentFuel95
        {
            get { return _currentFuel95; }
            set
            {
                if (_currentFuel95 == value) return;
                _currentFuel95 = value;
                OnPropertyChanged();
            }
        }

        private double _currentFuel98;
        public double CurrentFuel98
        {
            get { return _currentFuel98; }
            set
            {
                if (_currentFuel98 == value) return;
                _currentFuel98 = value;
                OnPropertyChanged();
            }
        }

        private double _currentFuelDiesel;
        public double CurrentFuelDiesel
        {
            get { return _currentFuelDiesel; }
            set
            {
                if (_currentFuelDiesel == value) return;
                _currentFuelDiesel = value;
                OnPropertyChanged();
            }
        }

        private double _limitMoney;
        public double LimitMoney
        {
            get { return _limitMoney; }
            set
            {
                if (_limitMoney == value) return;
                _limitMoney = value;
                OnPropertyChanged();
            }
        }

        private double _limitFuel92;
        public double LimitFuel92
        {
            get { return _limitFuel92; }
            set
            {
                if (_limitFuel92 == value) return;
                _limitFuel92 = value;
                OnPropertyChanged();
            }
        }

        private double _limitFuel95;
        public double LimitFuel95
        {
            get { return _limitFuel95; }
            set
            {
                if (_limitFuel95 == value) return;
                _limitFuel95 = value;
                OnPropertyChanged();
            }
        }

        private double _limitFuel98;
        public double LimitFuel98
        {
            get { return _limitFuel98; }
            set
            {
                if (_limitFuel98 == value) return;
                _limitFuel98 = value;
                OnPropertyChanged();
            }
        }

        private double _limitFuelDiesel;
        public double LimitFuelDiesel
        {
            get { return _limitFuelDiesel; }
            set
            {
                if (_limitFuelDiesel == value) return;
                _limitFuelDiesel = value;
                OnPropertyChanged();
            }
        }

        private double _lowFuel92;
        public double LowFuel92
        {
            get { return _lowFuel92; }
            set
            {
                if (_lowFuel92 == value) return;
                _lowFuel92 = value;
                OnPropertyChanged();
            }
        }

        private double _lowFuel95;
        public double LowFuel95
        {
            get { return _lowFuel95; }
            set
            {
                if (_lowFuel95 == value) return;
                _lowFuel95 = value;
                OnPropertyChanged();
            }
        }

        private double _lowFuel98;
        public double LowFuel98
        {
            get { return _lowFuel98; }
            set
            {
                if (_lowFuel98 == value) return;
                _lowFuel98 = value;
                OnPropertyChanged();
            }
        }

        private double _lowFuelDiesel;
        public double LowFuelDiesel
        {
            get { return _lowFuelDiesel; }
            set
            {
                if (_lowFuelDiesel == value) return;
                _lowFuelDiesel = value;
                OnPropertyChanged();
            }
        }

        private double _criticalFuel92;
        public double CriticalFuel92
        {
            get { return _criticalFuel92; }
            set
            {
                if (_criticalFuel92 == value) return;
                _criticalFuel92 = value;
                OnPropertyChanged();
            }
        }

        private double _criticalFuel95;
        public double CriticalFuel95
        {
            get { return _criticalFuel95; }
            set
            {
                if (_criticalFuel95 == value) return;
                _criticalFuel95 = value;
                OnPropertyChanged();
            }
        }

        private double _criticalFuel98;
        public double CriticalFuel98
        {
            get { return _criticalFuel98; }
            set
            {
                if (_criticalFuel98 == value) return;
                _criticalFuel98 = value;
                OnPropertyChanged();
            }
        }

        private double _criticalFuelDiesel;
        public double CriticalFuelDiesel
        {
            get { return _criticalFuelDiesel; }
            set
            {
                if (_criticalFuelDiesel == value) return;
                _criticalFuelDiesel = value;
                OnPropertyChanged();
            }
        }

        private FSModel _fsModel;
        public FSModel FSModel
        {
            get { return _fsModel; }
            set
            {
                if (_fsModel == value) return;
                _fsModel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

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
