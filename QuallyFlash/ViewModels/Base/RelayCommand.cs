using System;
using System.Windows.Input;

namespace QuallyFlash
{
    public class RelayCommand : ICommand
    {
        private readonly Action action;
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }
}
