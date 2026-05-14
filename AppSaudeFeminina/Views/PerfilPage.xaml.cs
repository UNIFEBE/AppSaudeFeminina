namespace AppSaudeFeminina.Views;

public partial class PerfilPage : ContentPage
{
    public PerfilPage()
    {
        InitializeComponent();
    }

    private async void OnHojeTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }

    private async void OnCicloTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CicloPage));
    }

    private async void OnConteudosTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ConteudosPage));
    }
}
