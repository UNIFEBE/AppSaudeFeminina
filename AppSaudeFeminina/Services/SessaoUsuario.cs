namespace AppSaudeFeminina.Services
{
    /// <summary>
    /// Armazena os dados da sessão do usuário logado.
    /// Usa Preferences para persistir entre sessões do app.
    /// </summary>
    public static class SessaoUsuario
    {
        private const string UsuarioIdKey = "usuario_id";
        private const string UsuarioNomeKey = "usuario_nome";
        private const string UsuarioEmailKey = "usuario_email";

        public static string? UsuarioId
        {
            get => Preferences.Get(UsuarioIdKey, null);
            set => Preferences.Set(UsuarioIdKey, value ?? string.Empty);
        }

        public static string? Nome
        {
            get => Preferences.Get(UsuarioNomeKey, null);
            set => Preferences.Set(UsuarioNomeKey, value ?? string.Empty);
        }

        public static string? Email
        {
            get => Preferences.Get(UsuarioEmailKey, null);
            set => Preferences.Set(UsuarioEmailKey, value ?? string.Empty);
        }

        public static bool EstaLogado => !string.IsNullOrEmpty(UsuarioId);

        public static void Logout()
        {
            Preferences.Remove(UsuarioIdKey);
            Preferences.Remove(UsuarioNomeKey);
            Preferences.Remove(UsuarioEmailKey);
        }
    }
}
