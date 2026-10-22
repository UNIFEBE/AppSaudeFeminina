using AppSaudeFeminina.Services;

namespace AppSaudeFeminina.Views;

[QueryProperty(nameof(ArtigoId), "id")]
public partial class ArtigoDetalhePage : ContentPage
{
    private readonly ArtigoApiService _artigoService;
    private string _artigoId = string.Empty;

    public string ArtigoId
    {
        get => _artigoId;
        set
        {
            _artigoId = value;
            _ = CarregarArtigoAsync();
        }
    }

    public ArtigoDetalhePage()
    {
        InitializeComponent();
        _artigoService = new ArtigoApiService();
    }

    private async Task CarregarArtigoAsync()
    {
        if (string.IsNullOrEmpty(_artigoId)) return;

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        var artigo = await _artigoService.GetByIdAsync(_artigoId);

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        if (artigo == null)
        {
            await DisplayAlert("Conteúdo indisponível", "Este artigo não está disponível no momento.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        // Preenche cabeçalho
        LblTitulo.Text = artigo.Titulo;

        // Resumo
        LblResumo.Text = artigo.Resumo;
        ResumoCard.IsVisible = true;

        // Conteúdo HTML
        if (!string.IsNullOrWhiteSpace(artigo.Conteudo))
        {
            var conteudoAjustado = artigo.Conteudo;

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                conteudoAjustado = conteudoAjustado
                    .Replace("https://localhost:7252", "http://10.0.2.2:5026")
                    .Replace("http://localhost:5026", "http://10.0.2.2:5026")
                    .Replace("https://localhost:5026", "http://10.0.2.2:5026");
            }

            System.Diagnostics.Debug.WriteLine($"[ArtigoDetalhe] HTML ajustado: {conteudoAjustado}");

            var html = WrapHtml(conteudoAjustado);
            HtmlSource.Html = html;
            ConteudoCard.IsVisible = true;
        }
    }

    private static string WrapHtml(string conteudo)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
            font-size: 15px;
            line-height: 1.7;
            color: #374151;
            padding: 20px;
            background: white;
        }}
        h1 {{ font-size: 22px; color: #C43A4A; margin: 16px 0 8px; }}
        h2 {{ font-size: 19px; color: #C43A4A; margin: 14px 0 6px; }}
        h3 {{ font-size: 17px; color: #C43A4A; margin: 12px 0 6px; }}
        p {{ margin-bottom: 12px; }}
        ul, ol {{ padding-left: 20px; margin-bottom: 12px; }}
        li {{ margin-bottom: 4px; }}
        img {{ max-width: 100%; height: auto; border-radius: 12px; margin: 12px 0; }}
        a {{ color: #C43A4A; }}
        strong {{ font-weight: 700; }}
    </style>
</head>
<body>
    {conteudo}
</body>
</html>";
    }

    private async void OnVoltarTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}/{nameof(EspacoSaber)}");
    }
}
