using CVExpress.ViewModels;

namespace CVExpress.Views;

public partial class ExperiencesPage : ContentPage
{
    private readonly ExperiencesViewModel _viewModel;

    public ExperiencesPage(ExperiencesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.ChargerAsync();
    }
}