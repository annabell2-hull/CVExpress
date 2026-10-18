using CVExpress.Models;
using CVExpress.Services;

namespace CVExpress.ViewModels;

public class ExperienceFormViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService _db;
    private int _cvId;
    private int _experienceId;
    private Experience? _experience;

    private string _poste = "";
    private string _entreprise = "";
    private string _periode = "";
    private string _erreur = "";

    public string Poste
    { get => _poste; set => SetField(ref _poste, value); }

    public string Entreprise
    { get => _entreprise; set => SetField(ref _entreprise, value); }

    public string Periode
    { get => _periode; set => SetField(ref _periode, value); }

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
    public bool PeutSupprimer => _experienceId != 0;

    public AsyncCommand EnregistrerCommand { get; }
    public AsyncCommand SupprimerCommand { get; }
    public AsyncCommand RetourCommand { get; }

    public ExperienceFormViewModel(DatabaseService db)
    {
        _db = db;
        EnregistrerCommand = new AsyncCommand(EnregistrerAsync);
        SupprimerCommand = new AsyncCommand(SupprimerAsync);
        RetourCommand = new AsyncCommand(() => Shell.Current.GoToAsync(".."));
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("cvId", out var c) &&
            int.TryParse(c?.ToString(), out var cvId))
            _cvId = cvId;

        if (query.TryGetValue("experienceId", out var e) &&
            int.TryParse(e?.ToString(), out var expId))
            _experienceId = expId;

        OnPropertyChanged(nameof(PeutSupprimer));

        if (_experienceId != 0) _ = ChargerAsync();
    }

    private async Task ChargerAsync()
    {
        _experience = await _db.ObtenirExperienceAsync(_experienceId);
        if (_experience == null) return;

        Poste = _experience.Poste;
        Entreprise = _experience.Entreprise;
        Periode = _experience.Periode;
    }

    private async Task EnregistrerAsync()
    {
        Erreur = "";

        if (string.IsNullOrWhiteSpace(Poste))
        {
            Erreur = "Le poste est obligatoire.";
            return;
        }
        if (string.IsNullOrWhiteSpace(Entreprise))
        {
            Erreur = "L'entreprise est obligatoire.";
            return;
        }

        var experience = _experience ?? new Experience { CvId = _cvId };
        experience.Poste = Poste.Trim();
        experience.Entreprise = Entreprise.Trim();
        experience.Periode = Periode.Trim();

        await _db.SauvegarderExperienceAsync(experience);
        await Shell.Current.GoToAsync("..");
    }

    private async Task SupprimerAsync()
    {
        if (_experience == null) return;

        var confirme = await Shell.Current.DisplayAlert("Supprimer",
            "Supprimer cette expérience ?", "Oui", "Non");
        if (!confirme) return;

        await _db.SupprimerExperienceAsync(_experience);
        await Shell.Current.GoToAsync("..");
    }
}