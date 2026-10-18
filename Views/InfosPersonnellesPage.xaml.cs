using CVExpress.ViewModels;

namespace CVExpress.Views;

public partial class InfosPersonnellesPage : ContentPage
{
    public InfosPersonnellesPage(InfosPersonnellesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}