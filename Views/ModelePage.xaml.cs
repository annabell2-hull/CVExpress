using CVExpress.ViewModels;

namespace CVExpress.Views;

public partial class ModelePage : ContentPage
{
    public ModelePage(ModeleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}