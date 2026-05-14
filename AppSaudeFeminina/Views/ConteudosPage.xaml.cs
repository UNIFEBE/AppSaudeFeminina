namespace AppSaudeFeminina.Views;

public partial class ConteudosPage : ContentPage
{
    public ConteudosPage()
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

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PerfilPage));
    }
}
