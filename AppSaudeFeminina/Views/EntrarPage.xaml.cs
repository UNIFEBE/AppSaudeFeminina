namespace AppSaudeFeminina;

public partial class EntrarPage : ContentPage
{
    public EntrarPage()
    {
        InitializeComponent();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAcessarAppClicked(object sender, EventArgs e)
    {
        // Lógica para acessar o app
        await DisplayAlert("Login", "Bem-vinda de volta!", "OK");
    }
}
