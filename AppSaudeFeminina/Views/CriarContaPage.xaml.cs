using System.Net.Http.Json;
using System.Text.Json;
using AppSaudeFeminina.Services;
using AppSaudeFeminina.Models;

namespace AppSaudeFeminina;

public partial class CriarContaPage : ContentPage
{
    // ── Estado ────────────────────────────────────────────────────────
    private bool    _cicloRegular        = false;
    private bool?   _usaAnticoncepcional = null;
    private int?    _desejaEngravidar    = null;
    private int     _objetivo            = 1;
    private string? _tipoSanguineo       = null;
    private string? _faseVida            = null;

    // ── HTTP ──────────────────────────────────────────────────────────
    private readonly UsuarioApiService _usuarioApiService = new();
    private readonly HttpClient        _httpClient;
    private readonly string            _usuariosBaseUrl;
    private readonly string            _cicloBaseUrl;

    private static readonly JsonSerializerOptions _jsonOptions = new()
        { PropertyNameCaseInsensitive = true };

    private static readonly Dictionary<string, int> _objetivoMap = new()
    {
        {"Monitorar ciclo",1},{"Engravidar",2},
        {"Evitar gravidez",3},{"Acompanhar sintomas",4},
    };
    private static readonly Dictionary<int, string> _objetivoMapInv = new()
    {
        {1,"Monitorar ciclo"},{2,"Engravidar"},
        {3,"Evitar gravidez"},{4,"Acompanhar sintomas"},
    };

    public CriarContaPage()
    {
        InitializeComponent();

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _httpClient = new HttpClient(handler);

        bool isAndroid = DeviceInfo.Platform == DevicePlatform.Android;
        _usuariosBaseUrl = isAndroid
            ? "http://10.0.2.2:5026/api/usuarios"
            : "https://localhost:7252/api/usuarios";
        _cicloBaseUrl = isAndroid
            ? "http://10.0.2.2:5026/api/ciclo/config"
            : "https://localhost:7252/api/ciclo/config";
    }

    // ── Pré-carrega ───────────────────────────────────────────────────
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try { await PreencherFormularioAsync(); }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EditPerfil] OnAppearing erro: {ex.Message}");
        }
    }

    private async Task PreencherFormularioAsync()
    {
        var cicloApiService = new CicloApiService();
        var tarefaUsuario = _usuarioApiService.GetUsuarioLogadoAsync();
        var tarefaCiclo   = cicloApiService.GetConfigAsync(SessaoUsuario.UsuarioId ?? string.Empty);

        await Task.WhenAll(tarefaUsuario, tarefaCiclo);

        var u = tarefaUsuario.Result;
        var c = tarefaCiclo.Result;

        if (u is not null)
        {
            EntryNome.Text     = u.Nome;
            EntryEmail.Text    = u.Email;
            EntryTelefone.Text = u.Telefone ?? string.Empty;
            EntryCidade.Text   = u.Cidade   ?? string.Empty;
            EntryAltura.Text   = u.Altura.HasValue ? $"{u.Altura:0.#}" : string.Empty;
            EntryPeso.Text     = u.Peso.HasValue   ? $"{u.Peso:0.#}"   : string.Empty;

            if (u.DataNascimento.HasValue)
                DatePickerNascimento.Date = u.DataNascimento.Value;

            _tipoSanguineo = u.TipoSanguineo;
            if (!string.IsNullOrWhiteSpace(u.TipoSanguineo))
                LblTipoSanguineo.Text = u.TipoSanguineo;

            _faseVida = u.FaseVida;
            if (!string.IsNullOrWhiteSpace(u.FaseVida))
                LblFaseVida.Text = u.FaseVida;

            _usaAnticoncepcional = u.UsaAnticoncepcional;
            AtualizarToggle(BtnContSim, BtnContNao, u.UsaAnticoncepcional == true);

            _desejaEngravidar = u.DesejaEngravidar;
            AtualizarToggle3(u.DesejaEngravidar);

            if (u.Objetivo.HasValue && _objetivoMapInv.TryGetValue(u.Objetivo.Value, out var objTexto))
            {
                LblObjetivoSelecionado.Text = objTexto;
                _objetivo = u.Objetivo.Value;
            }
        }

        if (c is not null)
        {
            EntryDuracaoCiclo.Text = c.DuracaoCiclo.ToString();
            EntryDuracaoFluxo.Text = c.DuracaoMenstruacao.ToString();
            DatePickerUltimaMenstruacao.Date = c.DataUltimaMenstruacao;
            _cicloRegular = c.CicloRegular;
            AtualizarToggle(BtnSim, BtnIrregular, c.CicloRegular);
        }
    }

    // ── Salvar ────────────────────────────────────────────────────────
    private async void OnConcluirClicked(object sender, EventArgs e)
    {
        var usuarioId = SessaoUsuario.UsuarioId;
        if (string.IsNullOrEmpty(usuarioId))
        {
            await DisplayAlertAsync("Erro", "Sessão expirada. Faça login novamente.", "OK");
            try { await Shell.Current.GoToAsync("//MainPage"); } catch { }
            return;
        }

        if (string.IsNullOrWhiteSpace(EntryNome.Text))
        {
            await DisplayAlertAsync("Atenção", "O nome não pode estar vazio.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(EntryEmail.Text) || !EntryEmail.Text.Contains('@'))
        {
            await DisplayAlertAsync("Atenção", "Informe um e-mail válido.", "OK");
            return;
        }

        decimal? altura    = decimal.TryParse(EntryAltura.Text,       out var a)  ? a  : null;
        decimal? peso      = decimal.TryParse(EntryPeso.Text,         out var p)  ? p  : null;
        int?  durCiclo     = int.TryParse(EntryDuracaoCiclo.Text,     out var dc) ? dc : null;
        int?  durFluxo     = int.TryParse(EntryDuracaoFluxo.Text,     out var df) ? df : null;

        // ── Validações de ciclo ───────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(EntryDuracaoCiclo.Text))
        {
            if (!int.TryParse(EntryDuracaoCiclo.Text, out var valCiclo) || valCiclo < 15 || valCiclo > 60)
            {
                await DisplayAlertAsync("Duração do Ciclo Inválida",
                    "A duração do ciclo deve ser um número inteiro entre 15 e 60 dias.", "OK");
                return;
            }
        }

        if (!string.IsNullOrWhiteSpace(EntryDuracaoFluxo.Text))
        {
            if (!int.TryParse(EntryDuracaoFluxo.Text, out var valFluxo) || valFluxo < 1 || valFluxo > 15)
            {
                await DisplayAlertAsync("Duração do Fluxo Inválida",
                    "A duração do fluxo (menstruação) deve ser um número inteiro entre 1 e 15 dias.", "OK");
                return;
            }
        }

        bool salvoComSucesso = false;
        string erroMsg = string.Empty;

        try
        {
            // 1. Dados básicos
            var updateDto = new
            {
                Nome                = EntryNome.Text.Trim(),
                Email               = EntryEmail.Text.Trim(),
                DataNascimento      = DatePickerNascimento.Date.HasValue
                                          ? (DateTime?)DatePickerNascimento.Date.Value : null,
                Telefone            = EntryTelefone.Text?.Trim(),
                Cidade              = EntryCidade.Text?.Trim(),
                Estado              = (string?)null,
                UsaAnticoncepcional = _usaAnticoncepcional,
                DesejaEngravidar    = _desejaEngravidar,
                Objetivo            = _objetivo,
            };

            var resp1 = await _httpClient.PutAsJsonAsync(
                $"{_usuariosBaseUrl}/{usuarioId}", updateDto, _jsonOptions);

            if (!resp1.IsSuccessStatusCode)
            {
                erroMsg = "Não foi possível salvar os dados pessoais.";
                System.Diagnostics.Debug.WriteLine(
                    $"[EditPerfil] PUT usuario erro: {await resp1.Content.ReadAsStringAsync()}");
            }
            else
            {
                // 2. Dados de saúde
                var saudeDto = new
                {
                    Peso          = peso,
                    Altura        = altura,
                    TipoSanguineo = _tipoSanguineo,
                    FaseVida      = _faseVida,
                };
                var resp2 = await _httpClient.PutAsJsonAsync(
                    $"{_usuariosBaseUrl}/{usuarioId}/dados-saude", saudeDto, _jsonOptions);
                if (!resp2.IsSuccessStatusCode)
                    System.Diagnostics.Debug.WriteLine("[EditPerfil] PUT dados-saude falhou (não crítico).");

                // 3. Ciclo config
                var cicloPayload = new
                {
                    UsuarioId             = Guid.Parse(usuarioId),
                    DataUltimaMenstruacao = DatePickerUltimaMenstruacao.Date ?? DateTime.Today,
                    DuracaoCiclo          = durCiclo ?? 28,
                    DuracaoMenstruacao    = durFluxo ?? 5,
                    CicloRegular          = _cicloRegular,
                };
                var resp3 = await _httpClient.PostAsJsonAsync(_cicloBaseUrl, cicloPayload, _jsonOptions);
                if (!resp3.IsSuccessStatusCode)
                    System.Diagnostics.Debug.WriteLine(
                        $"[EditPerfil] POST ciclo/config falhou: {await resp3.Content.ReadAsStringAsync()}");

                SessaoUsuario.Nome  = EntryNome.Text.Trim();
                SessaoUsuario.Email = EntryEmail.Text.Trim();
                salvoComSucesso = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EditPerfil] Exceção: {ex.Message}");
            erroMsg = "Não foi possível conectar ao servidor.";
        }

        // Feedback e navegação SEMPRE fora do try/catch
        if (!string.IsNullOrEmpty(erroMsg))
        {
            await DisplayAlertAsync("Erro", erroMsg, "OK");
            return;
        }

        if (salvoComSucesso)
        {
            await DisplayAlertAsync("Sucesso", "Perfil atualizado com sucesso!", "OK");
            // Navega para //Perfil (ShellContent com rota absoluta)
            try
            {
                await Shell.Current.GoToAsync("//Perfil");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditPerfil] Navegação: {ex.Message}");
                // Fallback: volta para HomePage se Perfil falhar
                try { await Shell.Current.GoToAsync("//HomePage"); } catch { }
            }
        }
    }

    // ── Toggles ───────────────────────────────────────────────────────

    private static readonly Color _corAtivo       = Color.FromArgb("#C43A4A");
    private static readonly Color _corBranca      = Colors.White;
    private static readonly Color _corTextoPadrao = Color.FromArgb("#3A1A1A");
    private static readonly Color _corBordaPadrao = Color.FromArgb("#FBD9E5");

    private void AtualizarToggle(Button b1, Button b2, bool ativarB1)
    {
        if (ativarB1) { Ativar(b1); Desativar(b2); }
        else          { Desativar(b1); Ativar(b2); }
    }

    private void AtualizarToggle3(int? valor)
    {
        Desativar(BtnEngSim); Desativar(BtnEngNao); Desativar(BtnEngTalvez);
        switch (valor)
        {
            case 1: Ativar(BtnEngSim);    break;
            case 0: Ativar(BtnEngNao);    break;
            case 2: Ativar(BtnEngTalvez); break;
        }
    }

    private static void Ativar(Button btn)
    {
        btn.BackgroundColor = _corAtivo;
        btn.TextColor       = _corBranca;
        btn.BorderWidth     = 0;
    }

    private static void Desativar(Button btn)
    {
        btn.BackgroundColor = _corBranca;
        btn.TextColor       = _corTextoPadrao;
        btn.BorderColor     = _corBordaPadrao;
        btn.BorderWidth     = 1;
    }

    private void OnCicloSelecionado(object sender, EventArgs e)
    {
        _cicloRegular = (Button)sender == BtnSim;
        AtualizarToggle(BtnSim, BtnIrregular, _cicloRegular);
    }

    private void OnAntiSelecionado(object sender, EventArgs e)
    {
        _usaAnticoncepcional = (Button)sender == BtnContSim;
        AtualizarToggle(BtnContSim, BtnContNao, _usaAnticoncepcional == true);
    }

    private void OnEngravidarSelecionado(object sender, EventArgs e)
    {
        var clicado = (Button)sender;
        if      (clicado == BtnEngSim)    _desejaEngravidar = 1;
        else if (clicado == BtnEngNao)    _desejaEngravidar = 0;
        else if (clicado == BtnEngTalvez) _desejaEngravidar = 2;
        AtualizarToggle3(_desejaEngravidar);
    }

    // ── Dropdowns ─────────────────────────────────────────────────────

    private void OnDropdownTapped(object sender, EventArgs e)
        => BorderDropdown.IsVisible = !BorderDropdown.IsVisible;

    private void OnObjetivoSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string sel)
        {
            LblObjetivoSelecionado.Text = sel;
            BorderDropdown.IsVisible    = false;
            if (_objetivoMap.TryGetValue(sel, out var val)) _objetivo = val;
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private void OnTipoSanguineoTapped(object sender, EventArgs e)
        => BorderTipoSanguineo.IsVisible = !BorderTipoSanguineo.IsVisible;

    private void OnTipoSanguineoSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string sel)
        {
            _tipoSanguineo              = sel;
            LblTipoSanguineo.Text       = sel;
            BorderTipoSanguineo.IsVisible = false;
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private void OnFaseVidaTapped(object sender, EventArgs e)
        => BorderFaseVida.IsVisible = !BorderFaseVida.IsVisible;

    private void OnFaseVidaSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string sel)
        {
            _faseVida                = sel;
            LblFaseVida.Text         = sel;
            BorderFaseVida.IsVisible = false;
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
