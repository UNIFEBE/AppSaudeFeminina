namespace AppSaudeFeminina.Views;

public partial class ConteudosPage : ContentPage
{
    public ConteudosPage()
    {
        InitializeComponent();
    }

    private async void OnHojeTapped(object sender, EventArgs e)
    {
        try { await Shell.Current.GoToAsync($"//{nameof(HomePage)}"); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Conteudos] Nav: {ex.Message}"); }
    }

    private async void OnCicloTapped(object sender, EventArgs e)
    {
        try { await Shell.Current.GoToAsync(nameof(CicloPage)); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Conteudos] Nav: {ex.Message}"); }
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        try { await Shell.Current.GoToAsync("//Perfil"); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Conteudos] Nav: {ex.Message}"); }
    }
}
