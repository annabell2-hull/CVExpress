using CVExpress.ViewModels;

namespace CVExpress.Views;

public partial class ExportPage : ContentPage
{
    public ExportPage(ExportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}