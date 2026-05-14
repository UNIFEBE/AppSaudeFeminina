namespace AppSaudeFeminina;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCriarContaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CriarContaPage));
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EntrarPage));
    }
}
