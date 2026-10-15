namespace CVExpress.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnSeConnecterClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Connexion",
            "La connexion sera branchée à l'étape suivante.", "OK");
    }

    private async void OnCreerCompteTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(RegisterPage));
}