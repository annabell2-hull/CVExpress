using Microsoft.Extensions.Logging;
using CVExpress.Services;
using CVExpress.ViewModels;
using CVExpress.Views;

namespace CVExpress;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<SessionService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<MesCvViewModel>();
        builder.Services.AddTransient<MesCvPage>();
        builder.Services.AddTransient<InfosPersonnellesViewModel>();
        builder.Services.AddTransient<InfosPersonnellesPage>();
        builder.Services.AddTransient<InfosPersonnellesViewModel>();
        builder.Services.AddTransient<InfosPersonnellesPage>();
        builder.Services.AddTransient<ExperiencesViewModel>();
        builder.Services.AddTransient<ExperiencesPage>();
        builder.Services.AddTransient<ExperienceFormViewModel>();
        builder.Services.AddTransient<ExperienceFormPage>();
        builder.Services.AddTransient<ModeleViewModel>();
        builder.Services.AddTransient<ModelePage>();
        builder.Services.AddSingleton<CvPdfService>();
        builder.Services.AddTransient<ApercuViewModel>();
        builder.Services.AddTransient<ApercuPage>();
        builder.Services.AddTransient<ExportViewModel>();
        builder.Services.AddTransient<ExportPage>();

        return builder.Build();
    }
}
