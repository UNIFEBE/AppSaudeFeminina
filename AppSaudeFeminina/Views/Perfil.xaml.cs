namespace AppSaudeFeminina.Views;

public partial class Perfil : ContentPage
{
    public Perfil()
    {
        InitializeComponent();
    }

    // Evento disparado ao clicar na engrenagem
    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        // 1. Torna o fundo escuro e o modal visíveis na tela
        OverlayEscuro.IsVisible = true;
        ModalOpcoes.IsVisible = true;

        // 2. Executa as duas animações ao mesmo tempo (Task.WhenAll)
        // O fundo vai para 60% de opacidade (0.6)
        // O modal sobe para a posição original (0)
        await Task.WhenAll(
            OverlayEscuro.FadeTo(0.6, 250, Easing.CubicOut),
            ModalOpcoes.TranslateTo(0, 0, 300, Easing.CubicOut)
        );
    }

    // Evento disparado ao clicar no botão "FECHAR" ou no fundo escuro
    private async void FecharModal_Clicked(object sender, EventArgs e)
    {
        // 1. Executa as animações de saída ao mesmo tempo
        // O fundo volta a ficar totalmente transparente (0)
        // O modal desce 400 pixels, saindo da tela
        await Task.WhenAll(
            OverlayEscuro.FadeTo(0, 250, Easing.CubicIn),
            ModalOpcoes.TranslateTo(0, 400, 300, Easing.CubicIn)
        );

        // 2. Esconde os elementos completamente só APÓS a animação terminar
        OverlayEscuro.IsVisible = false;
        ModalOpcoes.IsVisible = false;
    }

    private async void SairDaConta_Clicked(object sender, EventArgs e)
    {
        // As duas barras "//" indicam uma rota absoluta. 
        // Isso limpa a pilha de navegação e impede que o usuário use o botão "voltar".
        await Shell.Current.GoToAsync("//MainPage");
    }
    private async void CriarConta_Clicked(object sender, EventArgs e)
    {
        // O PushAsync "empilha" a CriarContaPage por cima da página atual.
        // Isso cria automaticamente a setinha de voltar no topo da tela.
        await Navigation.PushAsync(new CriarContaPage());
    }
}