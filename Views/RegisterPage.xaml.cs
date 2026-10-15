namespace CVExpress.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnSeConnecterTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}