using System.Windows.Input;

namespace XIVUniPF.Classes
{
    public class BasicCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Action CommandAction { get; set; }
        public Func<bool>? CanExecuteFunc { get; set; }

        public BasicCommand(Action action, Func<bool>? canExecute = null)
        {
            CommandAction = action ?? throw new ArgumentNullException(nameof(action));
            CanExecuteFunc = canExecute;
        }

        public void Execute(object? parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object? parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }
    }
}
