using AppSaudeFeminina.Models;
using AppSaudeFeminina.Services;

namespace AppSaudeFeminina.Views;

public partial class EspacoSaber : ContentPage
{
    private readonly ArtigoApiService _artigoService;
    private List<ArtigoModel> _artigos = new();

    // Mapeamento de categoria tag → ID do banco
    private readonly Dictionary<string, string> _categoriaIds = new()
    {
        { "Ciclo", "e73e8c24-4515-4701-8b04-36f85edae823" },
        { "Prevencao", "3745ed99-6750-43d7-924d-a4e965041ba5" },
        { "BemEstar", "0e7de60d-9aea-45ef-be0f-d327c767037c" },
        { "Gestacao", "dfdc7897-ce8c-41b8-9624-222d39e27dd7" }
    };

    private readonly Dictionary<string, string> _categoriaTitulos = new()
    {
        { "Ciclo", "Tópicos em Menstruação" },
        { "Prevencao", "Tópicos em Contracepção" },
        { "BemEstar", "Tópicos em Saúde Íntima" },
        { "Gestacao", "Tópicos em Gravidez" }
    };

    // Bindings
    public List<ArtigoModel> Artigos
    {
        get => _artigos;
        set
        {
            _artigos = value;
            OnPropertyChanged();
        }
    }

    public EspacoSaber()
    {
        InitializeComponent();
        BindingContext = this;
        _artigoService = new ArtigoApiService();

        // Carrega a primeira categoria automaticamente
        _ = CarregarArtigosAsync("Ciclo");
    }

    private async void OnTagTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter == null) return;
        string tagClicada = e.Parameter.ToString()!;

        // Reset visual das tags
        ResetarTag(BtnCiclo, ImgCiclo, LblCicloTop, LblCicloBot, "ciclo");
        ResetarTag(BtnPrevencao, ImgPrevencao, LblPrevencaoTop, LblPrevencaoBot, "prevencao");
        ResetarTag(BtnBemEstar, ImgBemEstar, LblBemEstarTop, LblBemEstarBot, "bem_estar");
        ResetarTag(BtnGestacao, ImgGestacao, LblGestacaoTop, LblGestacaoBot, "gestacao");

        // Ativar tag selecionada
        switch (tagClicada)
        {
            case "Ciclo":
                AtivarTag(BtnCiclo, ImgCiclo, LblCicloTop, LblCicloBot, "ciclo");
                break;
            case "Prevencao":
                AtivarTag(BtnPrevencao, ImgPrevencao, LblPrevencaoTop, LblPrevencaoBot, "prevencao");
                break;
            case "BemEstar":
                AtivarTag(BtnBemEstar, ImgBemEstar, LblBemEstarTop, LblBemEstarBot, "bem_estar");
                break;
            case "Gestacao":
                AtivarTag(BtnGestacao, ImgGestacao, LblGestacaoTop, LblGestacaoBot, "gestacao");
                break;
        }

        await CarregarArtigosAsync(tagClicada);
    }

    private async Task CarregarArtigosAsync(string tag)
    {
        if (!_categoriaIds.ContainsKey(tag)) return;

        // Atualiza título da seção
        LblTituloSecao.Text = _categoriaTitulos[tag];

        // Mostra loading
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        LblVazio.IsVisible = false;
        Artigos = new List<ArtigoModel>();

        // Busca da API
        var categoriaId = _categoriaIds[tag];
        var artigos = await _artigoService.GetByCategoriaAsync(categoriaId);

        // Atualiza UI
        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        if (artigos.Count == 0)
        {
            LblVazio.IsVisible = true;
        }
        else
        {
            Artigos = artigos;
        }
    }

    private async void OnArtigoTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is string artigoId)
        {
            await Shell.Current.GoToAsync($"{nameof(ArtigoDetalhePage)}?id={artigoId}");
        }
    }

    // ── Visual helpers ────────────────────────────────────────

    private void ResetarTag(Border btn, Image img, Label lblTop, Label lblBot, string iconName)
    {
        btn.Style = (Style)Resources["TagNormalStyle"];
        lblTop.Style = (Style)Resources["TagTextTopNormal"];
        lblBot.Style = (Style)Resources["TagTextBotNormal"];

        switch (iconName)
        {
            case "ciclo": img.Source = "ciclo.svg"; break;
            case "prevencao": img.Source = "prevencao.svg"; break;
            case "bem_estar": img.Source = "bem_estar.svg"; break;
            case "gestacao": img.Source = "gestacao.svg"; break;
        }
    }

    private void AtivarTag(Border btn, Image img, Label lblTop, Label lblBot, string iconName)
    {
        btn.Style = (Style)Resources["TagSelecionadaStyle"];
        lblTop.Style = (Style)Resources["TagTextTopSelecionada"];
        lblBot.Style = (Style)Resources["TagTextBotSelecionada"];

        switch (iconName)
        {
            case "ciclo": img.Source = "ciclo_branco.svg"; break;
            case "prevencao": img.Source = "prevencao_branco.svg"; break;
            case "bem_estar": img.Source = "bem_estar_branco.svg"; break;
            case "gestacao": img.Source = "gestacao_branco.svg"; break;
        }
    }

    // ── Navbar ────────────────────────────────────────────────

    private async void OnHojeTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync($"//{nameof(HomePage)}");

    private async void OnCicloTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(CicloPage));

    private async void OnPerfilTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(Perfil));
}
