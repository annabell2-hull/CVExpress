using System.Windows.Input;

namespace CVExpress.ViewModels;

public class AsyncCommand : ICommand
{
    private readonly Func<Task> _executer;
    private readonly Func<bool>? _peutExecuter;
    private bool _enCours;

    public AsyncCommand(Func<Task> executer, Func<bool>? peutExecuter = null)
    {
        _executer = executer;
        _peutExecuter = peutExecuter;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parametre)
        => !_enCours && (_peutExecuter?.Invoke() ?? true);

    public async void Execute(object? parametre)
    {
        _enCours = true;
        Rafraichir();
        try { await _executer(); }
        finally
        {
            _enCours = false;
            Rafraichir();
        }
    }

    public void Rafraichir()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
