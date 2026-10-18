using CVExpress.Views;

namespace CVExpress;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(InfosPersonnellesPage),
                              typeof(InfosPersonnellesPage));
        Routing.RegisterRoute(nameof(ExperiencesPage),
                              typeof(ExperiencesPage));
        Routing.RegisterRoute(nameof(ExperienceFormPage),
                              typeof(ExperienceFormPage));
        Routing.RegisterRoute(nameof(ModelePage), typeof(ModelePage));
        Routing.RegisterRoute(nameof(ApercuPage), typeof(ApercuPage));
        Routing.RegisterRoute(nameof(ExportPage), typeof(ExportPage));
    }
}