using System.Collections.ObjectModel;
using CVExpress.Models;
using CVExpress.Services;
using CVExpress.Views;

namespace CVExpress.ViewModels;

public class ApercuViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService _db;
    private int _cvId;

    private string _nomComplet = "";
    private string _profession = "";
    private string _contact = "";
    private string _aPropos = "";
    private string _formation = "";
    private string _competences = "";
    private Color _accent = Colors.Black;

    public ObservableCollection<Experience> Experiences { get; } = new();

    public string NomComplet
    { get => _nomComplet; set => SetField(ref _nomComplet, value); }

    public string Profession
    { get => _profession; set => SetField(ref _profession, value); }

    public string Contact
    { get => _contact; set => SetField(ref _contact, value); }

    public string APropos
    { get => _aPropos; set => SetField(ref _aPropos, value); }

    public string Formation
    { get => _formation; set => SetField(ref _formation, value); }

    public string Competences
    { get => _competences; set => SetField(ref _competences, value); }

    public Color Accent
    { get => _accent; set => SetField(ref _accent, value); }

    public AsyncCommand ModifierCommand { get; }
    public AsyncCommand ExporterCommand { get; }
    public AsyncCommand RetourCommand { get; }

    public ApercuViewModel(DatabaseService db)
    {
        _db = db;

        ModifierCommand = new AsyncCommand(() =>
            Shell.Current.GoToAsync(
                $"{nameof(InfosPersonnellesPage)}?cvId={_cvId}"));
        ExporterCommand = new AsyncCommand(() =>
            Shell.Current.GoToAsync($"{nameof(ExportPage)}?cvId={_cvId}"));
        RetourCommand = new AsyncCommand(() => Shell.Current.GoToAsync(".."));
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("cvId", out var valeur) &&
            int.TryParse(valeur?.ToString(), out var id))
        {
            _cvId = id;
            _ = ChargerAsync();
        }
    }

    public async Task ChargerAsync()
    {
        if (_cvId == 0) return;

        var cv = await _db.ObtenirCvAsync(_cvId);
        if (cv == null) return;
        var liste = await _db.ListerExperiencesAsync(_cvId);

        NomComplet = $"{cv.Prenom} {cv.Nom}".Trim();
        Profession = cv.Profession;
        Contact = string.Join("  |  ",
            new[] { cv.Telephone, cv.Email, cv.Ville }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
        APropos = cv.APropos;
        Formation = cv.Formation;
        Competences = cv.Competences;
        Accent = cv.Modele switch
        {
            "Moderne" => Color.FromArgb("#1F6174"),
            "Sobre" => Color.FromArgb("#6B7280"),
            _ => Colors.Black
        };

        Experiences.Clear();
        foreach (var e in liste) Experiences.Add(e);
    }
}