using AppSaudeFeminina.Services;

namespace AppSaudeFeminina.Views;

public partial class Perfil : ContentPage
{
    private readonly UsuarioApiService _usuarioApiService = new();
    private readonly CicloApiService   _cicloApiService   = new();

    public Perfil()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await CarregarPerfilAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Perfil] OnAppearing erro: {ex.Message}");
        }
    }

    private async Task CarregarPerfilAsync()
    {
        var tarefaUsuario = _usuarioApiService.GetUsuarioLogadoAsync();
        var tarefaCiclo   = _cicloApiService.GetConfigAsync(SessaoUsuario.UsuarioId ?? string.Empty);

        await Task.WhenAll(tarefaUsuario, tarefaCiclo);

        var usuario = tarefaUsuario.Result;
        var ciclo   = tarefaCiclo.Result;

        if (usuario is not null)
        {
            LabelNome.Text  = usuario.Nome;
            LabelEmail.Text = usuario.Email;

            LabelPeso.Text = usuario.Peso.HasValue
                ? $"{usuario.Peso:0.#}kg" : "—";

            var localizacao = string.Join(", ",
                new[] { usuario.Cidade, usuario.Estado }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));
            LabelLocalizacao.Text = string.IsNullOrEmpty(localizacao) ? "—" : localizacao;

            LabelTipoSanguineo.Text = string.IsNullOrWhiteSpace(usuario.TipoSanguineo)
                ? "— Tipo sanguíneo"
                : $"{usuario.TipoSanguineo} · Tipo sanguíneo";

            LabelFaseVida.Text = string.IsNullOrWhiteSpace(usuario.FaseVida)
                ? "— Fase da vida" : usuario.FaseVida;

            LabelAltura.Text = usuario.Altura.HasValue
                ? $"{usuario.Altura:0.#} cm" : "—";
        }
        else
        {
            LabelNome.Text  = SessaoUsuario.Nome  ?? "—";
            LabelEmail.Text = SessaoUsuario.Email ?? "—";
        }

        LabelCicloMedio.Text = ciclo is not null
            ? $"{ciclo.DuracaoCiclo}d" : "28d";
    }

    // ── Modal ─────────────────────────────────────────────────────────

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        try
        {
            OverlayEscuro.IsVisible = true;
            ModalOpcoes.IsVisible   = true;
            await Task.WhenAll(
                OverlayEscuro.FadeToAsync(0.6, 250, Easing.CubicOut),
                ModalOpcoes.TranslateToAsync(0, 0, 300, Easing.CubicOut));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Perfil] OnSettingsClicked: {ex.Message}");
        }
    }

    private async void FecharModal_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Task.WhenAll(
                OverlayEscuro.FadeToAsync(0, 250, Easing.CubicIn),
                ModalOpcoes.TranslateToAsync(0, 400, 300, Easing.CubicIn));
            OverlayEscuro.IsVisible = false;
            ModalOpcoes.IsVisible   = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Perfil] FecharModal: {ex.Message}");
            OverlayEscuro.IsVisible = false;
            ModalOpcoes.IsVisible   = false;
        }
    }

    private async void SairDaConta_Clicked(object sender, EventArgs e)
    {
        try
        {
            SessaoUsuario.Logout();
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Perfil] SairDaConta: {ex.Message}");
        }
    }

    private async void CriarConta_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(CriarContaPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Perfil] CriarConta: {ex.Message}");
        }
    }

    // ── Navbar ────────────────────────────────────────────────────────

    private async void OnHojeTapped(object sender, TappedEventArgs e)
    {
        try { await Shell.Current.GoToAsync($"//{nameof(HomePage)}"); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Perfil] Nav: {ex.Message}"); }
    }

    private async void OnCicloTapped(object sender, TappedEventArgs e)
    {
        try { await Shell.Current.GoToAsync(nameof(CicloPage)); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Perfil] Nav: {ex.Message}"); }
    }

    private async void OnConteudosTapped(object sender, TappedEventArgs e)
    {
        try { await Shell.Current.GoToAsync(nameof(EspacoSaber)); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Perfil] Nav: {ex.Message}"); }
    }
}
