using System;
using System.Windows.Input;

namespace FillingStation.Helpers
{
    public class Command : ICommand
    {
        private readonly Action _action;
        public Command(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;
    }
}
