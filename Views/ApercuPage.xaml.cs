using CVExpress.ViewModels;

namespace CVExpress.Views;

public partial class ApercuPage : ContentPage
{
    private readonly ApercuViewModel _viewModel;

    public ApercuPage(ApercuViewModel viewModel)
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