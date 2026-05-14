using AppSaudeFeminina.Views;

namespace AppSaudeFeminina;

public partial class HomePage : ContentPage
{
    private string _especialidadeSelecionada = "Ginecologia";

    // Mapa especialidade → (Border, Label)
    private Dictionary<string, (Border btn, Label lbl)> _especialidades = null!;

    public HomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Inicializa o mapa após InitializeComponent
        _especialidades = new()
        {
            { "Ginecologia",  (BtnGinecologia,  LabelGinecologia)  },
            { "Mastologia",   (BtnMastologia,   LabelMastologia)   },
            { "Clínico",      (BtnClinico,      LabelClinico)      },
            { "Obstetrícia",  (BtnObstetricia,  LabelObstetricia)  },
        };
    }

    // ── Modal Agendar Consulta ────────────────────────────────────

    private void OnAgendarConsultaTapped(object sender, TappedEventArgs e)
    {
        SelecionarEspecialidade("Ginecologia"); // reset para padrão
        ModalConsulta.IsVisible = true;
    }

    private void OnFecharModalConsulta(object sender, TappedEventArgs e)
    {
        ModalConsulta.IsVisible = false;
    }

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
            btn.BackgroundColor = ativo ? Color.FromArgb("#7C3AED") : Colors.White;
            btn.Stroke          = new SolidColorBrush(ativo ? Colors.Transparent : Color.FromArgb("#E2E8F0"));
            btn.StrokeThickness = ativo ? 0 : 1.5;
            lbl.TextColor       = ativo ? Colors.White : Color.FromArgb("#4C1D95");
        }
    }

    private async void OnConfirmarAgendamento(object sender, EventArgs e)
    {
        var data = DatePickerConsulta.Date;
        var dataStr = data.HasValue
            ? data.Value.ToString("dd/MM/yyyy")
            : "data não selecionada";

        ModalConsulta.IsVisible = false;
        await DisplayAlert("Agendamento Confirmado",
            $"{_especialidadeSelecionada} agendado para {dataStr}.", "OK");
    }

    // ── Navbar ────────────────────────────────────────────────────

    private async void OnCicloTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(CicloPage));

    private async void OnConteudosTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(EspacoSaber));

    private async void OnPerfilTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(Perfil));
}
