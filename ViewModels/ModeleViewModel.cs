using CVExpress.Models;
using CVExpress.Services;
using CVExpress.Views;

namespace CVExpress.ViewModels;

public class ModeleViewModel : BaseViewModel, IQueryAttributable
{
    private static readonly Brush Actif =
        new SolidColorBrush(Color.FromArgb("#1F6174"));
    private static readonly Brush Inactif =
        new SolidColorBrush(Color.FromArgb("#D1D5DB"));

    private readonly DatabaseService _db;
    private int _cvId;
    private Cv? _cv;
    private string _modele = "Classique";

    public string Modele
    {
        get => _modele;
        set
        {
            if (!SetField(ref _modele, value)) return;
            OnPropertyChanged(nameof(BordureClassique));
            OnPropertyChanged(nameof(BordureModerne));
            OnPropertyChanged(nameof(BordureSobre));
        }
    }

    public Brush BordureClassique => Modele == "Classique" ? Actif : Inactif;
    public Brush BordureModerne => Modele == "Moderne" ? Actif : Inactif;
    public Brush BordureSobre => Modele == "Sobre" ? Actif : Inactif;

    public AsyncCommand<string> ChoisirCommand { get; }
    public AsyncCommand ApercuCommand { get; }
    public AsyncCommand RetourCommand { get; }

    public ModeleViewModel(DatabaseService db)
    {
        _db = db;

        ChoisirCommand = new AsyncCommand<string>(nom =>
        {
            if (!string.IsNullOrEmpty(nom)) Modele = nom;
            return Task.CompletedTask;
        });

        ApercuCommand = new AsyncCommand(ApercuAsync);
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

    private async Task ChargerAsync()
    {
        _cv = await _db.ObtenirCvAsync(_cvId);
        if (_cv == null) return;
        Modele = string.IsNullOrEmpty(_cv.Modele) ? "Classique" : _cv.Modele;
    }

    private async Task ApercuAsync()
    {
        if (_cv != null)
        {
            _cv.Modele = Modele;
            await _db.SauvegarderCvAsync(_cv);
        }

        await Shell.Current.GoToAsync(
    $"{nameof(ApercuPage)}?cvId={_cvId}");
    }
}