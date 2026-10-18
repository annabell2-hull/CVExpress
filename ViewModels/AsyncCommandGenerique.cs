using System.Windows.Input;

namespace CVExpress.ViewModels;

public class AsyncCommand<T> : ICommand where T : class
{
    private readonly Func<T?, Task> _executer;
    private bool _enCours;

    public AsyncCommand(Func<T?, Task> executer)
        => _executer = executer;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parametre) => !_enCours;

    public async void Execute(object? parametre)
    {
        _enCours = true;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        try { await _executer(parametre as T); }
        finally
        {
            _enCours = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}