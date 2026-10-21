using AppSaudeFeminina.Models;
using AppSaudeFeminina.Services;
using AppSaudeFeminina.Views;
using Microsoft.Maui.Controls.Shapes;

namespace AppSaudeFeminina;

public partial class HomePage : ContentPage
{
    private string _especialidadeSelecionada = "Ginecologia";
    private Dictionary<string, (Border btn, Label lbl)> _especialidades = null!;

    private readonly ConsultaApiService _consultaService = new();
    private readonly CicloApiService    _cicloService    = new();

    public HomePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Saudação
        var nome = SessaoUsuario.Nome ?? string.Empty;
        var primeiroNome = nome.Split(' ')[0];
        LabelOla.Text = string.IsNullOrWhiteSpace(primeiroNome) ? "Olá!" : $"Olá, {primeiroNome}";

        _especialidades = new()
        {
            { "Ginecologia", (BtnGinecologia, LabelGinecologia) },
            { "Mastologia",  (BtnMastologia,  LabelMastologia)  },
            { "Clínico",     (BtnClinico,     LabelClinico)     },
            { "Obstetrícia", (BtnObstetricia, LabelObstetricia) },
        };

        try { await Task.WhenAll(CarregarCicloAsync(), CarregarConsultasAsync()); }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Home] OnAppearing: {ex.Message}");
        }
    }

    // ── Status do Ciclo ───────────────────────────────────────────────

    private async Task CarregarCicloAsync()
    {
        var usuarioId = SessaoUsuario.UsuarioId;
        if (string.IsNullOrEmpty(usuarioId))
        {
            AtualizarCardCicloSemDados();
            return;
        }

        var config = await _cicloService.GetConfigAsync(usuarioId);

        if (config is null)
        {
            AtualizarCardCicloSemDados();
            return;
        }

        // ── Cálculos ──────────────────────────────────────────────────
        var hoje          = DateTime.Today;
        var ultimaMenst   = config.DataUltimaMenstruacao.Date;
        var duracaoCiclo  = config.DuracaoCiclo > 0 ? config.DuracaoCiclo : 28;
        var duracaoFluxo  = Math.Min(config.DuracaoMenstruacao > 0 ? config.DuracaoMenstruacao : 5,
                                     duracaoCiclo - 1); // fluxo nunca maior que o ciclo

        // Dia atual do ciclo (1-based)
        int diaCiclo = (int)(hoje - ultimaMenst).TotalDays + 1;

        // Ajusta caso hoje seja de um ciclo futuro (passou o ciclo sem registrar)
        while (diaCiclo > duracaoCiclo) diaCiclo -= duracaoCiclo;
        if (diaCiclo < 1) diaCiclo = 1;

        // Ovulação: ocorre a ~14 dias do FIM do ciclo (regra de Ogino-Knaus).
        // Para ciclos curtos (< 21 dias) a ovulação pode coincidir com a menstruação —
        // nesse caso usamos o centro do ciclo como estimativa.
        int diaOvulacao  = Math.Max(duracaoCiclo - 14, duracaoFluxo + 1);
        int inicioFertil = Math.Max(diaOvulacao - 2, duracaoFluxo + 1);
        int fimFertil    = Math.Min(diaOvulacao + 2, duracaoCiclo);

        string fase;
        string detalhe;

        if (diaCiclo <= duracaoFluxo)
        {
            fase    = "Fase Menstrual";
            int diasRestantes = duracaoFluxo - diaCiclo + 1;
            detalhe = diasRestantes == 1
                ? "Último dia da menstruação"
                : $"Menstruação • {diasRestantes} dias restantes";
        }
        else if (diaCiclo < inicioFertil)
        {
            fase    = "Fase Folicular";
            int diasParaFertil = inicioFertil - diaCiclo;
            detalhe = diasParaFertil == 1
                ? "Fase Folicular • Janela fértil amanhã"
                : $"Fase Folicular • Janela fértil em {diasParaFertil} dias";
        }
        else if (diaCiclo <= fimFertil)
        {
            fase    = "Fase Ovulatória";
            detalhe = diaCiclo == diaOvulacao
                ? "Ovulatória • Pico de fertilidade hoje"
                : "Ovulatória • Janela fértil ativa";
        }
        else
        {
            fase = "Fase Lútea";
            int diasProxCiclo = duracaoCiclo - diaCiclo + 1;
            detalhe = diasProxCiclo <= 1
                ? "Lútea • Próximo ciclo amanhã"
                : $"Fase Lútea • Próximo ciclo em {diasProxCiclo} dias";
        }

        // Progresso (0–100 %)
        double progresso = Math.Clamp((double)diaCiclo / duracaoCiclo, 0.01, 1.0);
        int pct   = (int)(progresso * 100);
        int resto = 100 - pct;

        // Atualiza UI
        LabelDiaCiclo.Text  = $"Dia {diaCiclo} do Ciclo";
        LabelFaseCiclo.Text = detalhe;

        // Barra de progresso: coluna 0 = preenchida, coluna 1 = restante
        GridProgresso.ColumnDefinitions.Clear();
        GridProgresso.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(pct,   GridUnitType.Star) });
        GridProgresso.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(resto, GridUnitType.Star) });
    }

    private void AtualizarCardCicloSemDados()
    {
        LabelDiaCiclo.Text  = "Configure seu ciclo";
        LabelFaseCiclo.Text = "Acesse Perfil › Editar para configurar";
        GridProgresso.ColumnDefinitions.Clear();
        GridProgresso.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1,  GridUnitType.Star) });
        GridProgresso.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(99, GridUnitType.Star) });
    }

    // ── Consultas ─────────────────────────────────────────────────────

    private async Task CarregarConsultasAsync()
    {
        var usuarioId = SessaoUsuario.UsuarioId;
        if (string.IsNullOrEmpty(usuarioId)) return;

        var consultas = await _consultaService.GetByUsuarioAsync(usuarioId);
        RenderizarConsultas(consultas);
    }

    private void RenderizarConsultas(List<ConsultaModel> consultas)
    {
        ListaConsultas.Children.Clear();

        var ordenadas = consultas
            .OrderBy(c => c.Status == "Cancelada" ? 1 : 0)
            .ThenBy(c => c.DataConsulta)
            .ToList();

        LabelSemConsultas.IsVisible = ordenadas.Count == 0;

        foreach (var c in ordenadas)
            ListaConsultas.Children.Add(CriarCardConsulta(c));
    }

    private View CriarCardConsulta(ConsultaModel c)
    {
        Color badgeBg = c.Status switch
        {
            "Confirmada" => Color.FromArgb("#DCFCE7"),
            "Cancelada"  => Color.FromArgb("#FEE2E2"),
            "Concluída"  => Color.FromArgb("#DBEAFE"),
            _            => Color.FromArgb("#F3F4F6"),
        };
        Color badgeText = c.Status switch
        {
            "Confirmada" => Color.FromArgb("#16A34A"),
            "Cancelada"  => Color.FromArgb("#DC2626"),
            "Concluída"  => Color.FromArgb("#2563EB"),
            _            => Color.FromArgb("#6B7280"),
        };
        string icone = c.Especialidade switch
        {
            "Ginecologia" => "🩺",
            "Mastologia"  => "🎗️",
            "Obstetrícia" => "🤰",
            _             => "🏥",
        };

        var card = new Border
        {
            StrokeShape     = new RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            Padding         = new Thickness(16, 14),
            Shadow          = new Shadow
            {
                Brush   = new SolidColorBrush(Color.FromArgb("#E7A48C")),
                Offset  = new Point(0, 4),
                Radius  = 8,
                Opacity = 0.25f,
            },
        };

        var layout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            ColumnSpacing = 12,
        };

        var iconeBorder = new Border
        {
            StrokeShape     = new RoundRectangle { CornerRadius = 12 },
            BackgroundColor = Color.FromArgb("#FBF4EB"),
            HeightRequest   = 44, WidthRequest = 44,
            StrokeThickness = 0,
            VerticalOptions = LayoutOptions.Center,
            Content = new Label
            {
                Text = icone, FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions   = LayoutOptions.Center,
            },
        };
        Grid.SetColumn(iconeBorder, 0);
        layout.Children.Add(iconeBorder);

        var info = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
        info.Children.Add(new Label { Text = c.Especialidade, FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#3A1A1A") });
        info.Children.Add(new Label { Text = c.DataConsulta.ToString("dd/MM/yyyy"), FontSize = 13, TextColor = Color.FromArgb("#6B7280") });
        Grid.SetColumn(info, 1);
        layout.Children.Add(info);

        var badge = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 20 },
            BackgroundColor = badgeBg, StrokeThickness = 0,
            Padding = new Thickness(10, 4), VerticalOptions = LayoutOptions.Center,
            Content = new Label { Text = c.Status, FontSize = 11, TextColor = badgeText, FontAttributes = FontAttributes.Bold },
        };
        Grid.SetColumn(badge, 2);
        layout.Children.Add(badge);

        card.Content = layout;
        return card;
    }

    // ── Modal Agendar Consulta ────────────────────────────────────────

    private void OnAgendarConsultaTapped(object sender, TappedEventArgs e)
    {
        SelecionarEspecialidade("Ginecologia");
        ModalConsulta.IsVisible = true;
    }

    private void OnFecharModalConsulta(object sender, TappedEventArgs e)
        => ModalConsulta.IsVisible = false;

    private void OnEspecialidadeTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is string especialidade)
            SelecionarEspecialidade(especialidade);
    }

    private void SelecionarEspecialidade(string especialidade)
    {
        _especialidadeSelecionada = especialidade;
        foreach (var (key, (btn, lbl)) in _especialidades)
        {
            bool ativo = key == especialidade;
            btn.BackgroundColor = ativo ? Color.FromArgb("#C43A4A") : Colors.White;
            btn.Stroke          = new SolidColorBrush(ativo ? Colors.Transparent : Color.FromArgb("#FBD9E5"));
            btn.StrokeThickness = ativo ? 0 : 1.5;
            lbl.TextColor       = ativo ? Colors.White : Color.FromArgb("#C43A4A");
        }
    }

    private async void OnConfirmarAgendamento(object sender, EventArgs e)
    {
        try
        {
            var usuarioId = SessaoUsuario.UsuarioId;
            if (string.IsNullOrEmpty(usuarioId)) { ModalConsulta.IsVisible = false; return; }

            var data = DatePickerConsulta.Date ?? DateTime.Today;

            if (data.Date <= DateTime.Today)
            {
                ModalConsulta.IsVisible = false;
                await DisplayAlertAsync("Data inválida",
                    "O agendamento deve ser feito com pelo menos um dia de antecedência. Escolha uma data a partir de amanhã.",
                    "OK");
                return;
            }

            var request = new CreateConsultaRequest
            {
                UsuarioId     = Guid.Parse(usuarioId),
                Especialidade = _especialidadeSelecionada,
                DataConsulta  = data,
                Status        = "Confirmada",
            };

            ModalConsulta.IsVisible = false;

            var criada = await _consultaService.CreateAsync(request);
            if (criada is not null)
            {
                await CarregarConsultasAsync();
                await DisplayAlertAsync("Agendado",
                    $"{_especialidadeSelecionada} confirmado para {data:dd/MM/yyyy}.", "OK");
            }
            else
            {
                await DisplayAlertAsync("Erro", "Não foi possível agendar a consulta.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Home] Agendamento: {ex.Message}");
            await DisplayAlertAsync("Erro", "Não foi possível conectar ao servidor.", "OK");
        }
    }

    // ── Navbar ────────────────────────────────────────────────────────

    private async void OnCicloTapped(object sender, EventArgs e)
    {
        try { await Shell.Current.GoToAsync(nameof(CicloPage)); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Home] Nav: {ex.Message}"); }
    }

    private async void OnConteudosTapped(object sender, EventArgs e)
    {
        try { await Shell.Current.GoToAsync(nameof(EspacoSaber)); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Home] Nav: {ex.Message}"); }
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        try { await Shell.Current.GoToAsync("//Perfil"); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Home] Nav: {ex.Message}"); }
    }
}
