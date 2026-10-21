using System.Net.Http.Json;
using AppSaudeFeminina.Services;

namespace AppSaudeFeminina;

public partial class EntrarPage : ContentPage
{
    public EntrarPage()
    {
        InitializeComponent();
    }

    private async void OnAcessarAppClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var senha = SenhaEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            await DisplayAlert("Erro", "Preencha e-mail e senha.", "OK");
            return;
        }

        try
        {
            var resultado = await LoginViaApi(email, senha);
            if (resultado is not null)
            {
                SessaoUsuario.UsuarioId = resultado.Id;
                SessaoUsuario.Nome      = resultado.Nome;
                SessaoUsuario.Email     = resultado.Email;
                try { await Shell.Current.GoToAsync($"//{nameof(HomePage)}"); }
                catch (Exception navEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[Login] Nav: {navEx.Message}");
                }
            }
            else
            {
                await DisplayAlert("Erro", "E-mail ou senha incorretos.", "Tentar novamente");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Login] Erro: {ex.Message}");
            await DisplayAlert("Erro", "Não foi possível conectar ao servidor.", "OK");
        }
    }

    private async Task<LoginResponse?> LoginViaApi(string email, string senha)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        using var client = new HttpClient(handler);

        string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5026/api/auth/login"
            : "https://localhost:7252/api/auth/login";

        var payload = new { Email = email, Senha = senha };
        var response = await client.PostAsJsonAsync(baseUrl, payload);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<LoginResponse>(
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return null;
    }

    private class LoginResponse
    {
        public string Id    { get; set; } = string.Empty;
        public string Nome  { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
