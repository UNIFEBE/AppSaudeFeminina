namespace AppSaudeFeminina;

public partial class EntrarPage : ContentPage
{
    private const string EmailValido = "maria.silva@gmail.com";
    private const string SenhaValida = "123456";

    public EntrarPage()
    {
        InitializeComponent();
    }

    private async void OnAcessarAppClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var senha = SenhaEntry.Text;

        if (email == EmailValido && senha == SenhaValida)
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
        else
        {
            await DisplayAlert("Erro", "E-mail ou senha incorretos.", "Tentar novamente");
        }
    }
}
