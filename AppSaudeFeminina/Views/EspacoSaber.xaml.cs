namespace AppSaudeFeminina.Views;

public partial class EspacoSaber : ContentPage
{
    public EspacoSaber()
    {
        InitializeComponent();
    }

    private void OnTagTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter == null) return;
        string tagClicada = e.Parameter.ToString();

        // 1. Volta todas as tags para branco e ícones para escuros
        // Passando a string base da imagem: "ciclo", "prevencao", etc.
        ResetarTag(BtnCiclo, ImgCiclo, LblCicloTop, LblCicloBot, "ciclo");
        ResetarTag(BtnPrevencao, ImgPrevencao, LblPrevencaoTop, LblPrevencaoBot, "prevencao");
        ResetarTag(BtnBemEstar, ImgBemEstar, LblBemEstarTop, LblBemEstarBot, "bem_estar");
        ResetarTag(BtnGestacao, ImgGestacao, LblGestacaoTop, LblGestacaoBot, "gestacao");

        // NOVO: Esconde todas as sessões de conteúdo
        SecaoCiclo.IsVisible = false;
        SecaoPrevencao.IsVisible = false;
        SecaoBemEstar.IsVisible = false;
        SecaoGestacao.IsVisible = false;

        // 2. Aplica rosa e ícone branco na tag clicada
        switch (tagClicada)
        {
            case "Ciclo":
                AtivarTag(BtnCiclo, ImgCiclo, LblCicloTop, LblCicloBot, "ciclo");
                SecaoCiclo.IsVisible = true; // Mostra a sessão do Ciclo
                break;
            case "Prevencao":
                AtivarTag(BtnPrevencao, ImgPrevencao, LblPrevencaoTop, LblPrevencaoBot, "prevencao");
                SecaoPrevencao.IsVisible = true; // Mostra a sessão de Prevenção
                break;
            case "BemEstar":
                AtivarTag(BtnBemEstar, ImgBemEstar, LblBemEstarTop, LblBemEstarBot, "bem_estar");
                SecaoBemEstar.IsVisible = true; // Mostra a sessão de Bem-Estar
                break;
            case "Gestacao":
                AtivarTag(BtnGestacao, ImgGestacao, LblGestacaoTop, LblGestacaoBot, "gestacao");
                SecaoGestacao.IsVisible = true; // Mostra a sessão de Gestação
                break;
        }
    }

    // Método auxiliar: Define o estilo Branco (Normal) e ícone escuro
    private void ResetarTag(Border btn, Image img, Label lblTop, Label lblBot, string iconName)
    {
        btn.Style = (Style)Resources["TagNormalStyle"];
        lblTop.Style = (Style)Resources["TagTextTopNormal"];
        lblBot.Style = (Style)Resources["TagTextBotNormal"];

        // Define a imagem base (ex: ciclo.svg)
        switch (iconName)
        {
            case "ciclo":
                img.Source = "ciclo.svg";
                break;
            case "prevencao":
                img.Source = "prevencao.svg";
                break;
            case "bem_estar":
                img.Source = "bem_estar.svg";
                break;
            case "gestacao":
                img.Source = "gestacao.svg";
                break;
        }
    }

    // Método auxiliar: Define o estilo Rosa (Selecionado) e ícone branco
    private void AtivarTag(Border btn, Image img, Label lblTop, Label lblBot, string iconName)
    {
        btn.Style = (Style)Resources["TagSelecionadaStyle"];
        lblTop.Style = (Style)Resources["TagTextTopSelecionada"];
        lblBot.Style = (Style)Resources["TagTextBotSelecionada"];

        // Adiciona _selected para puxar a versão branca (ex: ciclo_selected.svg)
        switch (iconName)
        {
            case "ciclo":
                img.Source = "ciclo_branco.svg";
                break;
            case "prevencao":
                img.Source = "prevencao_branco.svg";
                break;
            case "bem_estar":
                img.Source = "bem_estar_branco.svg";
                break;
            case "gestacao":
                img.Source = "gestacao_branco.svg";
                break;
        }
    }

    // ── Navbar ────────────────────────────────────────────────────

    private async void OnHojeTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync($"//{nameof(HomePage)}");

    private async void OnCicloTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(CicloPage));

    private async void OnPerfilTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(Perfil));

}