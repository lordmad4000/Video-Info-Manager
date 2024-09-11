using System.Windows.Input;

namespace VideoInfoManager.Presentation.Wpf.Handlers;


public class CommandHandler : ICommand
{
    private readonly Action<object> _action;
    private readonly Predicate<object>? _canExecute;

    public CommandHandler(Action<object> action, Predicate<object>? canExecute)
    {
        _action = action;
        _canExecute = canExecute;
    }

    public CommandHandler(Action<object> action) : this(action, null)
    {
    }

    public event EventHandler? CanExecuteChanged
    {
        add 
        { 
            CommandManager.RequerySuggested += value; 
        }
        remove 
        { 
            CommandManager.RequerySuggested -= value; 
        }
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute is null
            ? true
            : _canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _action(parameter);
    }

}