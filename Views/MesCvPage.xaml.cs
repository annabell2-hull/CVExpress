using CVExpress.ViewModels;

namespace CVExpress.Views;

public partial class MesCvPage : ContentPage
{
    private readonly MesCvViewModel _viewModel;

    public MesCvPage(MesCvViewModel viewModel)
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