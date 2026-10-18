using CVExpress.ViewModels;

namespace CVExpress.Views;

public partial class ExperienceFormPage : ContentPage
{
    public ExperienceFormPage(ExperienceFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}