using Microsoft.Maui.Controls.Shapes;

namespace AppSaudeFeminina.Views;

public partial class CicloPage : ContentPage
{
    private DateTime _mesAtual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    private readonly DateTime _hoje = DateTime.Today;

    private bool _mostrarMenstruacao = false;
    private bool _mostrarFertil = false;

    // Dados do ciclo (editáveis pelo modal)
    private int _duracaoCiclo = 28;
    private int _duracaoFluxo = 5;
    private DateTime _inicioUltima = new DateTime(2026, 5, 14);

    private readonly HashSet<DateTime> _diasMenstruacao = new()
    {
        new DateTime(2026, 5, 13), new DateTime(2026, 5, 14), new DateTime(2026, 5, 15),
        new DateTime(2026, 5, 16), new DateTime(2026, 5, 17), new DateTime(2026, 5, 18)
    };

    private readonly HashSet<DateTime> _diasFertil = new()
    {
        new DateTime(2026, 5, 24), new DateTime(2026, 5, 25), new DateTime(2026, 5, 26),
        new DateTime(2026, 5, 27), new DateTime(2026, 5, 28), new DateTime(2026, 5, 29)
    };

    private static readonly Color CorMenstruacaoFundo = Color.FromArgb("#FCE7F3");
    private static readonly Color CorMenstruacaoTexto = Color.FromArgb("#EC4899");
    private static readonly Color CorFertilFundo      = Color.FromArgb("#FEF9C3");
    private static readonly Color CorFertilTexto      = Color.FromArgb("#CA8A04");
    private static readonly Color CorHojeFundo        = Color.FromArgb("#4C1D95");
    private static readonly Color CorPadraoTexto      = Color.FromArgb("#4C1D95");

    public CicloPage()
    {
        InitializeComponent();
        RenderizarCalendario();
        AtualizarBotoes();
        AtualizarCards();
    }

    // ── Modal ─────────────────────────────────────────────────────

    private void OnAbrirModalCiclo(object sender, TappedEventArgs e)
    {
        EntryDuracao.Text       = _duracaoCiclo.ToString();
        EntryFluxo.Text         = _duracaoFluxo.ToString();
        DatePickerInicio.Date   = _inicioUltima;
        ModalOverlay.IsVisible  = true;
    }

    private void OnFecharModal(object sender, TappedEventArgs e)
    {
        ModalOverlay.IsVisible = false;
    }

    private void OnSalvarCiclo(object sender, EventArgs e)
    {
        if (int.TryParse(EntryDuracao.Text, out int dur) && dur > 0)
            _duracaoCiclo = dur;

        if (int.TryParse(EntryFluxo.Text, out int flu) && flu > 0)
            _duracaoFluxo = flu;

        _inicioUltima = DatePickerInicio.Date ?? _inicioUltima;

        // Recalcula dias de menstruação e fértil com base nos novos dados
        RecalcularDias();
        RenderizarCalendario();
        AtualizarCards();

        ModalOverlay.IsVisible = false;
    }

    private void RecalcularDias()
    {
        _diasMenstruacao.Clear();
        for (int i = 0; i < _duracaoFluxo; i++)
            _diasMenstruacao.Add(_inicioUltima.AddDays(i));

        // Janela fértil: ~14 dias antes do próximo ciclo, por 6 dias
        var proximoCiclo = _inicioUltima.AddDays(_duracaoCiclo);
        var inicioFertil = proximoCiclo.AddDays(-14);
        _diasFertil.Clear();
        for (int i = 0; i < 6; i++)
            _diasFertil.Add(inicioFertil.AddDays(i));
    }

    private void AtualizarCards()
    {
        LabelStatus.Text = $"{_duracaoCiclo} dias • {_duracaoFluxo} dias de fluxo";

        var proximaCiclo = _inicioUltima.AddDays(_duracaoCiclo);
        int diasRestantes = (proximaCiclo - _hoje).Days;
        LabelProximoEvento.Text = diasRestantes > 0
            ? $"Próxima menstruação em {diasRestantes} dia{(diasRestantes > 1 ? "s" : "")}."
            : diasRestantes == 0
                ? "Menstruação prevista para hoje."
                : "Menstruação em atraso.";
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

            var data   = new DateTime(_mesAtual.Year, _mesAtual.Month, dia);
            var celula_ = CriarCelulaDia(data, dia);
            Grid.SetRow(celula_, row);
            Grid.SetColumn(celula_, col);
            GridDias.Children.Add(celula_);
        }
    }

    private Border CriarCelulaDia(DateTime data, int dia)
    {
        bool isHoje        = data == _hoje;
        bool isMenstruacao = _mostrarMenstruacao && _diasMenstruacao.Contains(data);
        bool isFertil      = _mostrarFertil      && _diasFertil.Contains(data);

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
        else if (isFertil)
        {
            fundo = CorFertilFundo;
            texto = CorFertilTexto;
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

    // ── Botões toggle ─────────────────────────────────────────────

    private void OnMenstruacaoTapped(object sender, TappedEventArgs e)
    {
        _mostrarMenstruacao = !_mostrarMenstruacao;
        AtualizarBotoes();
        RenderizarCalendario();
    }

    private void OnFertilTapped(object sender, TappedEventArgs e)
    {
        _mostrarFertil = !_mostrarFertil;
        AtualizarBotoes();
        RenderizarCalendario();
    }

    private void AtualizarBotoes()
    {
        BtnMenstruacao.BackgroundColor = _mostrarMenstruacao ? CorMenstruacaoFundo : Colors.White;
        BtnMenstruacao.Stroke          = new SolidColorBrush(Color.FromArgb("#EC4899"));
        BtnMenstruacao.StrokeThickness = _mostrarMenstruacao ? 2.5 : 2;

        BtnFertil.BackgroundColor = _mostrarFertil ? CorFertilFundo : Colors.White;
        BtnFertil.Stroke          = new SolidColorBrush(_mostrarFertil
            ? Color.FromArgb("#CA8A04")
            : Color.FromArgb("#D1D5DB"));
        BtnFertil.StrokeThickness = _mostrarFertil ? 2.5 : 2;
    }

    // ── Navbar ────────────────────────────────────────────────────

    private async void OnHojeTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync($"//{nameof(HomePage)}");

    private async void OnConteudosTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(EspacoSaber));

    private async void OnPerfilTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(Perfil));
}
