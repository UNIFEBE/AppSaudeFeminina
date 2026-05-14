using AppSaudeFeminina.Views;

namespace AppSaudeFeminina
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EntrarPage), typeof(EntrarPage));
            Routing.RegisterRoute(nameof(CriarContaPage), typeof(CriarContaPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(CicloPage), typeof(CicloPage));
            Routing.RegisterRoute(nameof(ConteudosPage), typeof(ConteudosPage));
            Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));
        }
    }
}
