using System.Collections.ObjectModel;
using System.Globalization;
using CVExpress.Models;
using CVExpress.Services;
using CVExpress.Views;

namespace CVExpress.ViewModels;

public class CvAffichage
{
    public int Id { get; set; }
    public string Titre { get; set; } = "";
    public string ModifieLe { get; set; } = "";
}

public class MesCvViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private readonly SessionService _session;
    private static readonly CultureInfo Fr = new("fr-FR");
    private bool _vide;

    public ObservableCollection<CvAffichage> Cvs { get; } = new();

    public bool Vide
    { get => _vide; set => SetField(ref _vide, value); }

    public AsyncCommand CreerCommand { get; }
    public AsyncCommand DeconnexionCommand { get; }
    public AsyncCommand<CvAffichage> ModifierCommand { get; }
    public AsyncCommand<CvAffichage> ApercuCommand { get; }

    public MesCvViewModel(DatabaseService db, SessionService session)
    {
        _db = db;
        _session = session;

        CreerCommand = new AsyncCommand(CreerAsync);
        DeconnexionCommand = new AsyncCommand(DeconnexionAsync);

        ModifierCommand = new AsyncCommand<CvAffichage>(cv =>
            cv == null
                ? Task.CompletedTask
                : Shell.Current.GoToAsync(
                    $"{nameof(InfosPersonnellesPage)}?cvId={cv.Id}"));

        // Provisoire : cet écran arrive plus tard
        ApercuCommand = new AsyncCommand<CvAffichage>(cv =>
    cv == null
        ? Task.CompletedTask
        : Shell.Current.GoToAsync(
            $"{nameof(ApercuPage)}?cvId={cv.Id}"));
    }

    public async Task ChargerAsync()
    {
        if (_session.UserId is not int userId) return;

        var liste = await _db.ListerCvAsync(userId);

        Cvs.Clear();
        foreach (var cv in liste)
        {
            Cvs.Add(new CvAffichage
            {
                Id = cv.Id,
                Titre = cv.Titre,
                ModifieLe = "Modifié le " +
                    cv.DateModification.ToString("d MMMM yyyy", Fr)
            });
        }
        Vide = Cvs.Count == 0;
    }

    private async Task CreerAsync()
    {
        if (_session.UserId is not int userId) return;

        var titre = await Shell.Current.DisplayPromptAsync(
            "Nouveau CV", "Donne un titre à ton CV :",
            "Créer", "Annuler", "Ex : CV Développeur", 50);

        if (string.IsNullOrWhiteSpace(titre)) return;

        var cv = await _db.AjouterCvAsync(new Cv
        {
            UserId = userId,
            Titre = titre.Trim(),
            Prenom = _session.Prenom,
            Nom = _session.Nom,
            Email = _session.Email
        });

        await Shell.Current.GoToAsync(
            $"{nameof(InfosPersonnellesPage)}?cvId={cv.Id}");
    }

    private async Task DeconnexionAsync()
    {
        _session.Fermer();
        Cvs.Clear();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}