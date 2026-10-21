using Microsoft.Maui.Controls.Shapes;
using AppSaudeFeminina.Models;
using AppSaudeFeminina.Services;

namespace AppSaudeFeminina.Views;

public partial class CicloPage : ContentPage
{
    private DateTime _mesAtual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    private readonly DateTime _hoje = DateTime.Today;

    private readonly CicloApiService _cicloApi = new();

    // Dados carregados da API
    private PrevisaoCicloModel? _previsao;
    private List<CicloRegistroModel> _registros = new();

    // Períodos para pintar no calendário
    private readonly HashSet<DateTime> _diasMenstruacao = new();
    private readonly HashSet<DateTime> _diasPrevisao = new();

    private static readonly Color CorMenstruacaoFundo = Color.FromArgb("#FCE7F3");
    private static readonly Color CorMenstruacaoTexto = Color.FromArgb("#EC4899");
    private static readonly Color CorPrevisaoFundo    = Color.FromArgb("#DBEAFE");
    private static readonly Color CorPrevisaoTexto    = Color.FromArgb("#2563EB");
    private static readonly Color CorHojeFundo        = Color.FromArgb("#4C1D95");
    private static readonly Color CorPadraoTexto      = Color.FromArgb("#4C1D95");

    public CicloPage()
    {
        InitializeComponent();
        RenderizarCalendario();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await CarregarDadosDaApi();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CicloPage] OnAppearing erro: {ex.Message}");
        }
    }

    // ── Carregar dados da API ─────────────────────────────────────

    private async Task CarregarDadosDaApi()
    {
        var usuarioId = SessaoUsuario.UsuarioId;
        if (string.IsNullOrEmpty(usuarioId))
        {
            LabelPrevisao.Text = "Faça login para ver sua previsão.";
            return;
        }

        try
        {
            // Carrega previsão (inclui últimos registros)
            _previsao = await _cicloApi.GetPrevisaoAsync(usuarioId);

            // Carrega todos os registros para o histórico
            _registros = await _cicloApi.GetCicloRegistrosAsync(usuarioId);

            AtualizarCardPrevisao();
            RecalcularDiasCalendario();
            RenderizarCalendario();
            RenderizarHistorico();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CicloPage] Erro ao carregar: {ex.Message}");
        }
    }

    private void AtualizarCardPrevisao()
    {
        if (_previsao is null || _previsao.TotalRegistros == 0)
        {
            LabelPrevisao.Text = "Registre seu primeiro período para ver a previsão.";
            LabelCicloMedio.Text = "-- dias";
            LabelDuracaoMedia.Text = "-- dias";
            LabelTotalRegistros.Text = "0";
            return;
        }

        if (_previsao.ProximaMenstruacao.HasValue)
        {
            var proxima = _previsao.ProximaMenstruacao.Value;
            var dias = _previsao.DiasParaProxima;

            if (dias > 0)
                LabelPrevisao.Text = $"Próxima menstruação em {dias} dia{(dias > 1 ? "s" : "")} ({proxima:dd/MM})";
            else if (dias == 0)
                LabelPrevisao.Text = "Menstruação prevista para hoje!";
            else
                LabelPrevisao.Text = $"Menstruação prevista há {Math.Abs(dias)} dia(s) atrás";
        }

        LabelCicloMedio.Text = $"{_previsao.CicloMedio} dias";
        LabelDuracaoMedia.Text = $"{_previsao.DuracaoMediaPeriodo} dias";
        LabelTotalRegistros.Text = _previsao.TotalRegistros.ToString();
    }

    private void RecalcularDiasCalendario()
    {
        _diasMenstruacao.Clear();
        _diasPrevisao.Clear();

        // Pinta os dias de menstruação registrados
        foreach (var reg in _registros)
        {
            var fim = reg.DataFim ?? reg.DataInicio.AddDays((_previsao?.DuracaoMediaPeriodo ?? 5) - 1);
            for (var d = reg.DataInicio; d <= fim; d = d.AddDays(1))
                _diasMenstruacao.Add(d.Date);
        }

        // Pinta os dias de previsão do próximo ciclo
        if (_previsao?.ProximaMenstruacao.HasValue == true)
        {
            var inicio = _previsao.ProximaMenstruacao.Value;
            var duracao = _previsao.DuracaoMediaPeriodo;
            for (int i = 0; i < duracao; i++)
                _diasPrevisao.Add(inicio.AddDays(i).Date);
        }
    }

    // ── Modal Registrar Período ───────────────────────────────────

    private void OnRegistrarPeriodoTapped(object sender, TappedEventArgs e)
    {
        DatePickerInicio.Date = _hoje;
        DatePickerFim.Date = _hoje;
        ModalOverlay.IsVisible = true;
    }

    private void OnFecharModal(object sender, TappedEventArgs e)
    {
        ModalOverlay.IsVisible = false;
    }

    private async void OnSalvarPeriodo(object sender, EventArgs e)
    {
        var usuarioId = SessaoUsuario.UsuarioId;
        if (string.IsNullOrEmpty(usuarioId))
        {
            ModalOverlay.IsVisible = false;
            return;
        }

        var dataInicio = DatePickerInicio.Date ?? _hoje;
        var dataFim    = DatePickerFim.Date;
        DateTime? fim  = (dataFim.HasValue && dataFim.Value > dataInicio) ? dataFim.Value : null;

        ModalOverlay.IsVisible = false;

        try
        {
            // 1. Salva o registro do período (ciclo_registro)
            var request = new CreateCicloRegistroRequest
            {
                UsuarioId  = usuarioId,
                DataInicio = dataInicio,
                DataFim    = fim
            };

            var result = await _cicloApi.CreateCicloRegistroAsync(request);

            if (result is not null)
            {
                // 2. Atualiza ciclo_config com a nova data_ultima_menstruacao (upsert)
                //    Mantém os valores já existentes de duração; se não houver config usa defaults.
                var configAtual = await _cicloApi.GetConfigAsync(usuarioId);

                var cicloConfigRequest = new CreateCicloConfigRequest
                {
                    UsuarioId             = usuarioId,
                    DataUltimaMenstruacao = dataInicio,          // data do novo período
                    DuracaoCiclo          = configAtual?.DuracaoCiclo      ?? 28,
                    DuracaoMenstruacao    = configAtual?.DuracaoMenstruacao ?? 5,
                    CicloRegular          = configAtual?.CicloRegular      ?? true,
                };

                await _cicloApi.SaveConfigAsync(cicloConfigRequest);

                // 3. Recarrega todos os dados da tela
                await CarregarDadosDaApi();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CicloPage] Erro ao salvar período: {ex.Message}");
        }
    }

    // ── Histórico ─────────────────────────────────────────────────

    private void RenderizarHistorico()
    {
        ListaHistorico.Children.Clear();

        if (_registros.Count == 0)
        {
            ListaHistorico.Children.Add(new Label
            {
                Text = "Nenhum período registrado ainda.",
                FontSize = 14,
                TextColor = Color.FromArgb("#9CA3AF"),
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 10)
            });
            return;
        }

        foreach (var reg in _registros)
        {
            var card = CriarCardHistorico(reg);
            ListaHistorico.Children.Add(card);
        }
    }

    private Border CriarCardHistorico(CicloRegistroModel reg)
    {
        var fimText = reg.DataFim.HasValue
            ? reg.DataFim.Value.ToString("dd/MM/yyyy")
            : "Em andamento";

        var duracaoText = reg.DuracaoPeriodo.HasValue
            ? $"{reg.DuracaoPeriodo} dias"
            : "---";

        var cicloText = reg.DuracaoCiclo.HasValue
            ? $"Ciclo: {reg.DuracaoCiclo} dias"
            : "";

        var content = new VerticalStackLayout { Spacing = 4 };

        content.Children.Add(new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label { Text = "\U0001F534", FontSize = 16, VerticalOptions = LayoutOptions.Center },
                new Label
                {
                    Text = $"{reg.DataInicio:dd/MM/yyyy}  →  {fimText}",
                    FontSize = 14, FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#3A1A1A"), VerticalOptions = LayoutOptions.Center
                }
            }
        });

        var detailsLayout = new HorizontalStackLayout { Spacing = 16 };
        detailsLayout.Children.Add(new Label
        {
            Text = $"Duração: {duracaoText}",
            FontSize = 12, TextColor = Color.FromArgb("#6B7280")
        });

        if (!string.IsNullOrEmpty(cicloText))
        {
            detailsLayout.Children.Add(new Label
            {
                Text = cicloText,
                FontSize = 12, TextColor = Color.FromArgb("#6B7280")
            });
        }

        content.Children.Add(detailsLayout);

        return new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            Padding = new Thickness(16, 12),
            Content = content,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#C56682")),
                Offset = new Point(0, 2),
                Radius = 8,
                Opacity = 0.1f
            }
        };
    }

    // ── Calendário ────────────────────────────────────────────────

    private void RenderizarCalendario()
    {
        var nomesMes = new[] { "JANEIRO","FEVEREIRO","MARÇO","ABRIL","MAIO","JUNHO",
                               "JULHO","AGOSTO","SETEMBRO","OUTUBRO","NOVEMBRO","DEZEMBRO" };
        LabelMes.Text = nomesMes[_mesAtual.Month - 1];
        LabelAno.Text = _mesAtual.Year.ToString();

        GridDias.Children.Clear();
        GridDias.RowDefinitions.Clear();

        int primeiroDia = (int)_mesAtual.DayOfWeek;
        int totalDias   = DateTime.DaysInMonth(_mesAtual.Year, _mesAtual.Month);
        int totalLinhas = (int)Math.Ceiling((primeiroDia + totalDias) / 7.0);

        for (int i = 0; i < totalLinhas; i++)
            GridDias.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        for (int celula = 0; celula < totalLinhas * 7; celula++)
        {
            int dia = celula - primeiroDia + 1;
            int col = celula % 7;
            int row = celula / 7;

            if (dia < 1 || dia > totalDias)
            {
                var vazio = new BoxView { Color = Colors.Transparent, HeightRequest = 38 };
                Grid.SetRow(vazio, row);
                Grid.SetColumn(vazio, col);
                GridDias.Children.Add(vazio);
                continue;
            }

            var data = new DateTime(_mesAtual.Year, _mesAtual.Month, dia);
            var celulaDia = CriarCelulaDia(data, dia);
            Grid.SetRow(celulaDia, row);
            Grid.SetColumn(celulaDia, col);
            GridDias.Children.Add(celulaDia);
        }
    }

    private Border CriarCelulaDia(DateTime data, int dia)
    {
        bool isHoje        = data == _hoje;
        bool isMenstruacao = _diasMenstruacao.Contains(data);
        bool isPrevisao    = _diasPrevisao.Contains(data);

        Color fundo = Colors.Transparent;
        Color texto = CorPadraoTexto;

        if (isHoje)
        {
            fundo = CorHojeFundo;
            texto = Colors.White;
        }
        else if (isMenstruacao)
        {
            fundo = CorMenstruacaoFundo;
            texto = CorMenstruacaoTexto;
        }
        else if (isPrevisao)
        {
            fundo = CorPrevisaoFundo;
            texto = CorPrevisaoTexto;
        }

        return new Border
        {
            StrokeShape     = new RoundRectangle { CornerRadius = 19 },
            BackgroundColor = fundo,
            StrokeThickness = 0,
            HeightRequest   = 38,
            WidthRequest    = 38,
            HorizontalOptions = LayoutOptions.Center,
            Margin  = new Thickness(0, 2),
            Content = new Label
            {
                Text             = dia.ToString(),
                FontSize         = 14,
                FontAttributes   = isHoje ? FontAttributes.Bold : FontAttributes.None,
                TextColor        = texto,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions   = LayoutOptions.Center,
            }
        };
    }

    // ── Navegação de mês ──────────────────────────────────────────

    private void OnMesAnteriorClicked(object sender, EventArgs e)
    {
        _mesAtual = _mesAtual.AddMonths(-1);
        RenderizarCalendario();
    }

    private void OnProximoMesClicked(object sender, EventArgs e)
    {
        _mesAtual = _mesAtual.AddMonths(1);
        RenderizarCalendario();
    }

    // ── Navbar ────────────────────────────────────────────────────

    private async void OnHojeTapped(object sender, TappedEventArgs e)
    {
        try { await Shell.Current.GoToAsync($"//{nameof(HomePage)}"); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Ciclo] Nav: {ex.Message}"); }
    }

    private async void OnConteudosTapped(object sender, TappedEventArgs e)
    {
        try { await Shell.Current.GoToAsync(nameof(EspacoSaber)); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Ciclo] Nav: {ex.Message}"); }
    }

    private async void OnPerfilTapped(object sender, TappedEventArgs e)
    {
        try { await Shell.Current.GoToAsync("//Perfil"); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Ciclo] Nav: {ex.Message}"); }
    }
}
