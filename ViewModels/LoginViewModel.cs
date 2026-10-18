using CVExpress.Services;
using CVExpress.Views;

namespace CVExpress.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private readonly SessionService _session;

    private string _email = "";
    private string _motDePasse = "";
    private string _erreur = "";

    public string Email
    { get => _email; set => SetField(ref _email, value); }

    public string MotDePasse
    { get => _motDePasse; set => SetField(ref _motDePasse, value); }

    public string Erreur
    {
        get => _erreur;
        set
        {
            if (SetField(ref _erreur, value))
                OnPropertyChanged(nameof(ErreurVisible));
        }
    }

    public bool ErreurVisible => !string.IsNullOrEmpty(Erreur);

    public AsyncCommand ConnexionCommand { get; }
    public AsyncCommand CreerCompteCommand { get; }

    public LoginViewModel(DatabaseService db, SessionService session)
    {
        _db = db;
        _session = session;
        ConnexionCommand = new AsyncCommand(ConnexionAsync);
        CreerCompteCommand = new AsyncCommand(
            () => Shell.Current.GoToAsync(nameof(RegisterPage)));
    }

    private async Task ConnexionAsync()
    {
        Erreur = "";

        if (string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrEmpty(MotDePasse))
        {
            Erreur = "Saisis ton e-mail et ton mot de passe.";
            return;
        }

        var utilisateur = await _db.ConnecterAsync(Email, MotDePasse);
        if (utilisateur == null)
        {
            Erreur = "E-mail ou mot de passe incorrect.";
            return;
        }

        _session.Ouvrir(utilisateur);
        MotDePasse = "";
        await Shell.Current.GoToAsync("//MesCvPage");
    }
}