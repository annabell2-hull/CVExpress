using System.Collections.ObjectModel;
using CVExpress.Models;
using CVExpress.Services;
using CVExpress.Views;

namespace CVExpress.ViewModels;

public class ExperiencesViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService _db;
    private int _cvId;
    private bool _vide;

    public ObservableCollection<Experience> Experiences { get; } = new();

    public bool Vide
    { get => _vide; set => SetField(ref _vide, value); }

    public AsyncCommand AjouterCommand { get; }
    public AsyncCommand ContinuerCommand { get; }
    public AsyncCommand RetourCommand { get; }
    public AsyncCommand<Experience> ModifierCommand { get; }

    public ExperiencesViewModel(DatabaseService db)
    {
        _db = db;

        AjouterCommand = new AsyncCommand(() =>
            Shell.Current.GoToAsync(
                $"{nameof(ExperienceFormPage)}?cvId={_cvId}&experienceId=0"));

        ModifierCommand = new AsyncCommand<Experience>(e =>
            e == null
                ? Task.CompletedTask
                : Shell.Current.GoToAsync(
                    $"{nameof(ExperienceFormPage)}?cvId={_cvId}&experienceId={e.Id}"));

        ContinuerCommand = new AsyncCommand(() =>
            Shell.Current.GoToAsync($"{nameof(ModelePage)}?cvId={_cvId}"));

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

        var liste = await _db.ListerExperiencesAsync(_cvId);
        Experiences.Clear();
        foreach (var e in liste) Experiences.Add(e);
        Vide = Experiences.Count == 0;
    }
}