using System.Net.Mail;
using CVExpress.Models;
using CVExpress.Services;
using CVExpress.Views;

namespace CVExpress.ViewModels;

public class InfosPersonnellesViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService _db;
    private int _cvId;
    private Cv? _cv;

    private string _prenom = "";
    private string _nom = "";
    private string _profession = "";
    private string _telephone = "";
    private string _email = "";
    private string _ville = "";
    private string _aPropos = "";
    private string _erreur = "";
    private string _formation = "";
    private string _competences = "";

    public string Formation
    { get => _formation; set => SetField(ref _formation, value); }

    public string Competences
    { get => _competences; set => SetField(ref _competences, value); }

    public string Prenom
    { get => _prenom; set => SetField(ref _prenom, value); }

    public string Nom
    { get => _nom; set => SetField(ref _nom, value); }

    public string Profession
    { get => _profession; set => SetField(ref _profession, value); }

    public string Telephone
    { get => _telephone; set => SetField(ref _telephone, value); }

    public string Email
    { get => _email; set => SetField(ref _email, value); }

    public string Ville
    { get => _ville; set => SetField(ref _ville, value); }

    public string APropos
    { get => _aPropos; set => SetField(ref _aPropos, value); }

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

    public AsyncCommand ContinuerCommand { get; }
    public AsyncCommand RetourCommand { get; }

    public InfosPersonnellesViewModel(DatabaseService db)
    {
        _db = db;
        ContinuerCommand = new AsyncCommand(ContinuerAsync);
        RetourCommand = new AsyncCommand(() => Shell.Current.GoToAsync(".."));
    }

    // Appelé par Shell avec les paramètres de navigation (?cvId=...)
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("cvId", out var valeur) &&
            int.TryParse(valeur?.ToString(), out var id))
        {
            _cvId = id;
            _ = ChargerAsync();
        }
    }

    private async Task ChargerAsync()
    {
        _cv = await _db.ObtenirCvAsync(_cvId);
        if (_cv == null) return;

        Prenom = _cv.Prenom;
        Nom = _cv.Nom;
        Profession = _cv.Profession;
        Telephone = _cv.Telephone;
        Email = _cv.Email;
        Ville = _cv.Ville;
        APropos = _cv.APropos;
        Formation = _cv.Formation;
        Competences = _cv.Competences;
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
        if (string.IsNullOrWhiteSpace(Profession))
            return "La profession est obligatoire.";

        if (!string.IsNullOrWhiteSpace(Telephone))
        {
            var chiffres = Telephone.Count(char.IsDigit);
            if (chiffres < 8 || chiffres > 15)
                return "Le numéro de téléphone n'est pas valide.";
        }

        if (!string.IsNullOrWhiteSpace(Email) && !EmailValide(Email))
            return "L'adresse e-mail n'est pas valide.";

        return null;
    }

    private async Task ContinuerAsync()
    {
        Erreur = "";

        var probleme = Valider();
        if (probleme != null)
        {
            Erreur = probleme;
            return;
        }

        if (_cv == null)
        {
            Erreur = "Ce CV est introuvable.";
            return;
        }

        _cv.Prenom = Prenom.Trim();
        _cv.Nom = Nom.Trim();
        _cv.Profession = Profession.Trim();
        _cv.Telephone = Telephone.Trim();
        _cv.Email = Email.Trim();
        _cv.Ville = Ville.Trim();
        _cv.APropos = APropos.Trim();
        _cv.Formation = Formation.Trim();
        _cv.Competences = Competences.Trim();

        await _db.SauvegarderCvAsync(_cv);

        await Shell.Current.GoToAsync(
     $"{nameof(ExperiencesPage)}?cvId={_cvId}");
    }
}
