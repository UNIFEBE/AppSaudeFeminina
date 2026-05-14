namespace AppSaudeFeminina;

public partial class CriarContaPage : ContentPage
{
    public CriarContaPage()
    {
        InitializeComponent();
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Cadastro", "Cadastro realizado com sucesso!", "OK");
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }
}
