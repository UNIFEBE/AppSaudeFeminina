using System.Net.Http.Json;
using System.Text.Json;

namespace AppSaudeFeminina.Services
{
    /// <summary>
    /// Modelo local com os dados de perfil retornados pela API.
    /// </summary>
    public class UsuarioPerfilModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public string? Telefone { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public string? TipoSanguineo { get; set; }
        public string? FaseVida { get; set; }
        public bool? UsaAnticoncepcional { get; set; }
        public int? DesejaEngravidar { get; set; }
        public int? Objetivo { get; set; }
    }

    public class UsuarioApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public UsuarioApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _httpClient = new HttpClient(handler);

            if (DeviceInfo.Platform == DevicePlatform.Android)
                _baseUrl = "http://10.0.2.2:5026/api/usuarios";
            else
                _baseUrl = "https://localhost:7252/api/usuarios";
        }

        /// <summary>
        /// Busca os dados completos do usuário pelo ID salvo na sessão.
        /// Retorna null se o usuário não estiver logado ou ocorrer um erro.
        /// </summary>
        public async Task<UsuarioPerfilModel?> GetUsuarioLogadoAsync()
        {
            var usuarioId = SessaoUsuario.UsuarioId;

            if (string.IsNullOrEmpty(usuarioId))
            {
                System.Diagnostics.Debug.WriteLine("[UsuarioApi] Nenhum usuário logado.");
                return null;
            }

            try
            {
                var url = $"{_baseUrl}/{usuarioId}";
                System.Diagnostics.Debug.WriteLine($"[UsuarioApi] GET {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[UsuarioApi] Erro HTTP: {response.StatusCode}");
                    return null;
                }

                var usuario = await response.Content.ReadFromJsonAsync<UsuarioPerfilModel>(_jsonOptions);
                System.Diagnostics.Debug.WriteLine($"[UsuarioApi] Perfil carregado: {usuario?.Nome}");
                return usuario;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UsuarioApi] ERRO ao buscar perfil: {ex.Message}");
                return null;
            }
        }
    }
}
