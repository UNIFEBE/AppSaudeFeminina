using System.Net.Http.Json;
using AppSaudeFeminina.Services;

namespace AppSaudeFeminina;

public partial class CriarContaPage : ContentPage
{
    // Estado dos toggles
    private bool? _cicloRegular = null;
    private bool? _usaAnticoncepcional = null;
    private int?  _desejaEngravidar = null;  // 1=Sim 0=Não 2=Talvez
    private int   _objetivo = 1;             // default: Monitorar ciclo

    private static readonly Dictionary<string, int> _objetivoMap = new()
    {
        { "Monitorar ciclo",     1 },
        { "Engravidar",          2 },
        { "Evitar gravidez",     3 },
        { "Acompanhar sintomas", 4 }
    };

    public CriarContaPage()
    {
        InitializeComponent();
    }

    // ── Handlers de toggle ──────────────────────────────────────────────

    private void OnCicloSelecionado(object sender, EventArgs e)
    {
        var botao = (Button)sender;
        bool selecionouSim = botao == BtnSim;
        _cicloRegular = selecionouSim;

        AtivarBotao(BtnSim,       selecionouSim,  "#C43A4A");
        AtivarBotao(BtnIrregular, !selecionouSim, "#C43A4A");
    }

    private void OnAntiSelecionado(object sender, EventArgs e)
    {
        var botao = (Button)sender;
        bool selecionouSim = botao == BtnContSim;
        _usaAnticoncepcional = selecionouSim;

        AtivarBotao(BtnContSim, selecionouSim,  "#C43A4A");
        AtivarBotao(BtnContNao, !selecionouSim, "#C43A4A");
    }

    private void OnEngravidarSelecionado(object sender, EventArgs e)
    {
        var botao = (Button)sender;

        if (botao == BtnEngSim)      _desejaEngravidar = 1;
        else if (botao == BtnEngNao) _desejaEngravidar = 0;
        else                         _desejaEngravidar = 2;

        AtivarBotao(BtnEngSim,    botao == BtnEngSim,    "#C43A4A");
        AtivarBotao(BtnEngNao,    botao == BtnEngNao,    "#C43A4A");
        AtivarBotao(BtnEngTalvez, botao == BtnEngTalvez, "#C43A4A");
    }

    private void OnDropdownTapped(object sender, EventArgs e)
        => BorderDropdown.IsVisible = !BorderDropdown.IsVisible;

    private void OnObjetivoSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selecionado)
        {
            LblObjetivoSelecionado.Text = selecionado;
            _objetivo = _objetivoMap.TryGetValue(selecionado, out var val) ? val : 1;
            BorderDropdown.IsVisible = false;
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    // ── Submit ──────────────────────────────────────────────────────────

    private async void OnConcluirClicked(object sender, TappedEventArgs e)
    {
        var nome  = NomeEntry.Text?.Trim();
        var email = EmailEntry.Text?.Trim();
        var senha = SenhaEntry.Text;

        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            await DisplayAlert("Atenção", "Nome, e-mail e senha são obrigatórios.", "OK");
            return;
        }

        decimal? altura = null;
        decimal? peso   = null;

        if (!string.IsNullOrWhiteSpace(AlturaEntry.Text) &&
            decimal.TryParse(AlturaEntry.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var alturaVal))
            altura = alturaVal;

        if (!string.IsNullOrWhiteSpace(PesoEntry.Text) &&
            decimal.TryParse(PesoEntry.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var pesoVal))
            peso = pesoVal;

        int duracaoCiclo  = 28;
        int duracaoMenstr = 5;

        if (!string.IsNullOrWhiteSpace(DuracaoCicloEntry.Text) &&
            int.TryParse(DuracaoCicloEntry.Text, out var dc))
            duracaoCiclo = dc;

        if (!string.IsNullOrWhiteSpace(DuracaoFluxoEntry.Text) &&
            int.TryParse(DuracaoFluxoEntry.Text, out var df))
            duracaoMenstr = df;

        try
        {
            var usuarioCriado = await CriarUsuarioAsync(
                nome, email, senha,
                (DateTime?)DataNascimentoPicker.Date,
                TelefoneEntry.Text?.Trim(),
                altura, peso,
                _usaAnticoncepcional,
                _desejaEngravidar,
                _objetivo);

            if (usuarioCriado is null)
            {
                await DisplayAlert("Erro", "Não foi possível criar a conta. Verifique os dados.", "OK");
                return;
            }

            SessaoUsuario.UsuarioId = usuarioCriado.Id.ToString();
            SessaoUsuario.Nome      = usuarioCriado.Nome;
            SessaoUsuario.Email     = usuarioCriado.Email;

            await SalvarCicloConfigAsync(
                usuarioCriado.Id,
                (DateTime?)UltimaMenstruacaoPicker.Date,
                duracaoCiclo,
                duracaoMenstr,
                _cicloRegular ?? true);

            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CriarConta] Erro: {ex.Message}");
            await DisplayAlert("Erro", "Não foi possível conectar ao servidor.", "OK");
        }
    }

    // ── Chamadas à API ──────────────────────────────────────────────────

    private async Task<UsuarioCriadoResponse?> CriarUsuarioAsync(
        string nome, string email, string senha,
        DateTime? dataNascimento, string? telefone,
        decimal? altura, decimal? peso,
        bool? usaAnticoncepcional, int? desejaEngravidar, int objetivo)
    {
        using var client = CriarHttpClient();

        var payload = new
        {
            Nome                = nome,
            Email               = email,
            Senha               = senha,
            DataNascimento      = dataNascimento,
            Telefone            = telefone,
            Altura              = altura,
            Peso                = peso,
            UsaAnticoncepcional = usaAnticoncepcional,
            DesejaEngravidar    = desejaEngravidar,
            Objetivo            = objetivo
        };

        var response = await client.PostAsJsonAsync(BaseUrl("api/usuarios"), payload);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UsuarioCriadoResponse>(
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var erro = await response.Content.ReadFromJsonAsync<ApiErroResponse>(
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await DisplayAlert("Erro", erro?.Message ?? "Erro ao criar conta.", "OK");
        return null;
    }

    private async Task SalvarCicloConfigAsync(
        Guid usuarioId, DateTime? ultimaMenstruacao,
        int duracaoCiclo, int duracaoMenstruacao, bool cicloRegular)
    {
        using var client = CriarHttpClient();

        var payload = new
        {
            UsuarioId             = usuarioId,
            DataUltimaMenstruacao = ultimaMenstruacao ?? DateTime.Today,
            DuracaoCiclo          = duracaoCiclo,
            DuracaoMenstruacao    = duracaoMenstruacao,
            CicloRegular          = cicloRegular
        };

        var response = await client.PostAsJsonAsync(BaseUrl("api/ciclo/config"), payload);

        if (!response.IsSuccessStatusCode)
            System.Diagnostics.Debug.WriteLine("[CriarConta] Aviso: ciclo config não salvo.");
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static HttpClient CriarHttpClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        return new HttpClient(handler);
    }

    private static string BaseUrl(string path)
    {
        string host = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5026"
            : "https://localhost:7252";
        return $"{host}/{path}";
    }

    private static void AtivarBotao(Button btn, bool ativo, string corAtiva)
    {
        if (ativo)
        {
            btn.BackgroundColor = Color.FromArgb(corAtiva);
            btn.TextColor       = Colors.White;
            btn.BorderWidth     = 0;
        }
        else
        {
            btn.BackgroundColor = Colors.White;
            btn.TextColor       = Color.FromArgb("#3A1A1A");
            btn.BorderColor     = Color.FromArgb("#FBD9E5");
            btn.BorderWidth     = 1;
        }
    }

    // ── Modelos de resposta ───────────────────────────────────────────────

    private class UsuarioCriadoResponse
    {
        public Guid   Id    { get; set; }
        public string Nome  { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private class ApiErroResponse
    {
        public string? Message { get; set; }
    }
}
