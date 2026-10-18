using System.Net.Mail;
using CVExpress.Services;

namespace CVExpress.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    private string _prenom = "";
    private string _nom = "";
    private string _email = "";
    private string _motDePasse = "";
    private string _confirmation = "";
    private string _erreur = "";

    public string Prenom
    { get => _prenom; set => SetField(ref _prenom, value); }

    public string Nom
    { get => _nom; set => SetField(ref _nom, value); }

    public string Email
    { get => _email; set => SetField(ref _email, value); }

    public string MotDePasse
    { get => _motDePasse; set => SetField(ref _motDePasse, value); }

    public string Confirmation
    { get => _confirmation; set => SetField(ref _confirmation, value); }

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

    public AsyncCommand InscrireCommand { get; }
    public AsyncCommand RetourCommand { get; }

    public RegisterViewModel(DatabaseService db)
    {
        _db = db;
        InscrireCommand = new AsyncCommand(InscrireAsync);
        RetourCommand = new AsyncCommand(() => Shell.Current.GoToAsync(".."));
    }

    private static bool EmailValide(string email)
    {
        try
        {
            var adresse = new MailAddress(email.Trim());
            return adresse.Address == email.Trim();
        }
        catch { return false; }
    }

    private string? Valider()
    {
        if (string.IsNullOrWhiteSpace(Prenom))
            return "Le prénom est obligatoire.";
        if (string.IsNullOrWhiteSpace(Nom))
            return "Le nom est obligatoire.";
        if (!EmailValide(Email))
            return "L'adresse e-mail n'est pas valide.";
        if (MotDePasse.Length < 8)
            return "Le mot de passe doit contenir au moins 8 caractères.";
        if (MotDePasse != Confirmation)
            return "Les mots de passe ne correspondent pas.";
        return null;
    }

    private async Task InscrireAsync()
    {
        Erreur = "";

        var probleme = Valider();
        if (probleme != null)
        {
            Erreur = probleme;
            return;
        }

        var utilisateur = await _db.InscrireAsync(
            Prenom.Trim(), Nom.Trim(), Email.Trim(), MotDePasse);

        if (utilisateur == null)
        {
            Erreur = "Un compte existe déjà avec cet e-mail.";
            return;
        }

        await Shell.Current.DisplayAlert("Compte créé",
            "Tu peux maintenant te connecter.", "OK");
        await Shell.Current.GoToAsync("..");
    }
}