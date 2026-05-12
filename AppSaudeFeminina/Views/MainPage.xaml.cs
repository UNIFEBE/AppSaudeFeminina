namespace AppSaudeFeminina;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCriarContaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CriarContaPage());
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EntrarPage());
    }
}
