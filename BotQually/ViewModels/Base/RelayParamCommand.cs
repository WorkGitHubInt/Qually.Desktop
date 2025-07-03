using System;
using System.Windows.Input;

namespace BotQually
{
    public class RelayParamCommand : ICommand
    {
        private readonly Action<object> action;
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayParamCommand(Action<object> action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }
    }
}
