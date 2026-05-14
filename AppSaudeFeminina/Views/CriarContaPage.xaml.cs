using AppSaudeFeminina.Views;

namespace AppSaudeFeminina;

public partial class CriarContaPage : ContentPage
{
    public CriarContaPage()
    {
        InitializeComponent();
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Cadastro", "Cadastro realizado com sucesso!", "OK");
    }

    private void OnCicloSelecionado(object sender, EventArgs e)
    {
        var botaoClicado = (Button)sender;

        // Cores padrão
        Color corRoxa = Color.FromArgb("#A259FF");
        Color corBranca = Colors.White;
        Color corTextoDesativado = Colors.Black; // ou #5D2E8E se preferir
        Color corBordaDesativada = Color.FromArgb("#E5E5E5");

        if (botaoClicado == BtnSim)
        {
            // Ativa o Sim
            BtnSim.BackgroundColor = corRoxa;
            BtnSim.TextColor = corBranca;
            BtnSim.BorderWidth = 0;

            // Desativa o Irregular
            BtnIrregular.BackgroundColor = corBranca;
            BtnIrregular.TextColor = corTextoDesativado;
            BtnIrregular.BorderColor = corBordaDesativada;
            BtnIrregular.BorderWidth = 1;
        }
        else if (botaoClicado == BtnIrregular)
        {
            // Ativa o Irregular
            BtnIrregular.BackgroundColor = corRoxa;
            BtnIrregular.TextColor = corBranca;
            BtnIrregular.BorderWidth = 0;

            // Desativa o Sim
            BtnSim.BackgroundColor = corBranca;
            BtnSim.TextColor = corTextoDesativado;
            BtnSim.BorderColor = corBordaDesativada;
            BtnSim.BorderWidth = 1;
        }
    }

    private void OnAntiSelecionado(object sender, EventArgs e)
    {
        var botaoClicado = (Button)sender;

        // Cores padrão
        Color corRoxa = Color.FromArgb("#A259FF");
        Color corBranca = Colors.White;
        Color corTextoDesativado = Colors.Black; // ou #5D2E8E se preferir
        Color corBordaDesativada = Color.FromArgb("#E5E5E5");

        if (botaoClicado == BtnContSim)
        {
            // Ativa o Sim
            BtnContSim.BackgroundColor = corRoxa;
            BtnContSim.TextColor = corBranca;
            BtnContSim.BorderWidth = 0;

            // Desativa o Não
            BtnContNao.BackgroundColor = corBranca;
            BtnContNao.TextColor = corTextoDesativado;
            BtnContNao.BorderColor = corBordaDesativada;
            BtnContNao.BorderWidth = 1;
        }
        else if (botaoClicado == BtnContNao)
        {
            // Ativa o Não
            BtnContNao.BackgroundColor = corRoxa;
            BtnContNao.TextColor = corBranca;
            BtnContNao.BorderWidth = 0;

            // Desativa o Sim
            BtnContSim.BackgroundColor = corBranca;
            BtnContSim.TextColor = corTextoDesativado;
            BtnContSim.BorderColor = corBordaDesativada;
            BtnContSim.BorderWidth = 1;
        }
    }

    private void OnEngravidarSelecionado(object sender, EventArgs e)
    {
        var botaoClicado = (Button)sender;

        // Cores padrão
        Color corRoxa = Color.FromArgb("#EC4899");
        Color corBranca = Colors.White;
        Color corTextoDesativado = Colors.Black; // ou #5D2E8E se preferir
        Color corBordaDesativada = Color.FromArgb("#E5E5E5");

        if (botaoClicado == BtnEngSim)
        {
            // Ativa o Sim
            BtnEngSim.BackgroundColor = corRoxa;
            BtnEngSim.TextColor = corBranca;
            BtnEngSim.BorderWidth = 0;

            // Desativa o Não
            BtnEngNao.BackgroundColor = corBranca;
            BtnEngNao.TextColor = corTextoDesativado;
            BtnEngNao.BorderColor = corBordaDesativada;
            BtnEngNao.BorderWidth = 1;

            // Desativa o Talvez
            BtnEngTalvez.BackgroundColor = corBranca;
            BtnEngTalvez.TextColor = corTextoDesativado;
            BtnEngTalvez.BorderColor = corBordaDesativada;
            BtnEngTalvez.BorderWidth = 1;
        }
        else if (botaoClicado == BtnEngNao)
        {
            // Ativa o Não
            BtnEngNao.BackgroundColor = corRoxa;
            BtnEngNao.TextColor = corBranca;
            BtnEngNao.BorderWidth = 0;

            // Desativa o Sim
            BtnEngSim.BackgroundColor = corBranca;
            BtnEngSim.TextColor = corTextoDesativado;
            BtnEngSim.BorderColor = corBordaDesativada;
            BtnEngSim.BorderWidth = 1;

            // Desativa o Talvez
            BtnEngTalvez.BackgroundColor = corBranca;
            BtnEngTalvez.TextColor = corTextoDesativado;
            BtnEngTalvez.BorderColor = corBordaDesativada;
            BtnEngTalvez.BorderWidth = 1;
        }
        else if(botaoClicado == BtnEngTalvez)
        {
            // Ativa o Não
            BtnEngTalvez.BackgroundColor = corRoxa;
            BtnEngTalvez.TextColor = corBranca;
            BtnEngTalvez.BorderWidth = 0;

            // Desativa o Sim
            BtnEngSim.BackgroundColor = corBranca;
            BtnEngSim.TextColor = corTextoDesativado;
            BtnEngSim.BorderColor = corBordaDesativada;
            BtnEngSim.BorderWidth = 1;

            // Desativa o Talvez
            BtnEngNao.BackgroundColor = corBranca;
            BtnEngNao.TextColor = corTextoDesativado;
            BtnEngNao.BorderColor = corBordaDesativada;
            BtnEngNao.BorderWidth = 1;

        }
    }
    private void OnDropdownTapped(object sender, EventArgs e)
    {
        BorderDropdown.IsVisible = !BorderDropdown.IsVisible;
    }

    // Quando o usuário clica em uma opção
    private void OnObjetivoSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selecionado)
        {
            // Atualiza o texto do campo "fake"
            LblObjetivoSelecionado.Text = selecionado;

            // Esconde a lista novamente
            BorderDropdown.IsVisible = false;

            // Limpa a seleção para permitir clicar na mesma opção de novo se necessário
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnConcluirClicked(object sender, EventArgs e)
    {
        var border = (Border)sender;

        // Mudou de ScaleTo para ScaleToAsync
        await border.ScaleToAsync(0.95, 100);
        await border.ScaleToAsync(1.0, 100);

        // Mudou de DisplayAlert para DisplayAlertAsync
        await Navigation.PushAsync(new EspacoSaber());
    }
}