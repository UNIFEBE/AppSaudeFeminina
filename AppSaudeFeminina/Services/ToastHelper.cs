using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace AppSaudeFeminina.Services;

public static class ToastHelper
{
    public static async Task MostrarAsync(string mensagem, ToastDuration duracao = ToastDuration.Short)
    {
        var toast = Toast.Make(mensagem, duracao, 14);
        await toast.Show();
    }
}
