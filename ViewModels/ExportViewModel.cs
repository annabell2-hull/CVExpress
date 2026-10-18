using CVExpress.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace CVExpress.ViewModels;

public class ExportViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService _db;
    private readonly CvPdfService _pdf;
    private int _cvId;

    private string _nomComplet = "";
    private string _profession = "";
    private string _statut = "";

    public string NomComplet
    { get => _nomComplet; set => SetField(ref _nomComplet, value); }

    public string Profession
    { get => _profession; set => SetField(ref _profession, value); }

    public string Statut
    { get => _statut; set => SetField(ref _statut, value); }

    public AsyncCommand ExporterCommand { get; }
    public AsyncCommand PartagerCommand { get; }
    public AsyncCommand EnregistrerCommand { get; }
    public AsyncCommand RetourCommand { get; }

    public ExportViewModel(DatabaseService db, CvPdfService pdf)
    {
        _db = db;
        _pdf = pdf;

        ExporterCommand = new AsyncCommand(ExporterAsync);
        PartagerCommand = new AsyncCommand(PartagerAsync);
        EnregistrerCommand = new AsyncCommand(EnregistrerAsync);
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
        var cv = await _db.ObtenirCvAsync(_cvId);
        if (cv == null) return;
        NomComplet = $"{cv.Prenom} {cv.Nom}".Trim();
        Profession = cv.Profession;
    }

    private async Task<string?> CreerPdfAsync()
    {
        var cv = await _db.ObtenirCvAsync(_cvId);
        if (cv == null) return null;

        var experiences = await _db.ListerExperiencesAsync(_cvId);
        var chemin = _pdf.Generer(cv, experiences);
        Statut = $"PDF prêt : {Path.GetFileName(chemin)}";
        return chemin;
    }

    private async Task OuvrirAsync(string chemin)
    {
        await Launcher.Default.OpenAsync(
            new OpenFileRequest("Mon CV", new ReadOnlyFile(chemin)));
    }

    private async Task ExporterAsync()
    {
        try
        {
            var chemin = await CreerPdfAsync();
            if (chemin == null) return;

            try { await OuvrirAsync(chemin); }
            catch
            {
                await Shell.Current.DisplayAlert("PDF créé",
                    "Ton CV est exporté en PDF. Utilise « Partager » " +
                    "pour l'envoyer ou l'enregistrer.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur",
                "Impossible de créer le PDF : " + ex.Message, "OK");
        }
    }

    private async Task PartagerAsync()
    {
        try
        {
            var chemin = await CreerPdfAsync();
            if (chemin == null) return;

            try
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Mon CV",
                    File = new ShareFile(chemin)
                });
            }
            catch
            {
                // Si le partage n'existe pas sur cet appareil, on ouvre le PDF
                await OuvrirAsync(chemin);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur",
                "Impossible de partager le CV : " + ex.Message, "OK");
        }
    }

    private async Task EnregistrerAsync()
    {
        var cv = await _db.ObtenirCvAsync(_cvId);
        if (cv != null) await _db.SauvegarderCvAsync(cv);

        await Shell.Current.DisplayAlert("Enregistré",
            "Ton CV est enregistré dans « Mes CV ».", "OK");
        await Shell.Current.GoToAsync("//MesCvPage");
    }
}