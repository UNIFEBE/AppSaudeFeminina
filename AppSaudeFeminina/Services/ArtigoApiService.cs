using System.Net.Http.Json;
using AppSaudeFeminina.Models;

namespace AppSaudeFeminina.Services
{
    public class ArtigoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ArtigoApiService()
        {
            var handler = new HttpClientHandler
            {
                // Ignora validação de certificado SSL em dev
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _httpClient = new HttpClient(handler);

            // Android emulator usa 10.0.2.2 para acessar o host (HTTP porta 5026)
            // Windows/iOS usa localhost HTTPS
            if (DeviceInfo.Platform == DevicePlatform.Android)
                _baseUrl = "http://10.0.2.2:5026/api/artigos";
            else
                _baseUrl = "https://localhost:7252/api/artigos";
        }

        public async Task<List<ArtigoModel>> GetByCategoriaAsync(string categoriaId)
        {
            try
            {
                var url = $"{_baseUrl}/categoria/{categoriaId}";
                System.Diagnostics.Debug.WriteLine($"[ArtigoApi] GET {url}");

                var artigos = await _httpClient.GetFromJsonAsync<List<ArtigoModel>>(url);

                // Exibe apenas artigos publicados/ativos
                var publicados = artigos?.Where(a => a.Publicado).ToList() ?? new List<ArtigoModel>();

                System.Diagnostics.Debug.WriteLine($"[ArtigoApi] Retornou {artigos?.Count ?? 0} artigos, {publicados.Count} publicados");
                return publicados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ArtigoApi] ERRO ao buscar artigos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ArtigoApi] Stack: {ex.StackTrace}");
                return new List<ArtigoModel>();
            }
        }

        public async Task<ArtigoModel?> GetByIdAsync(string id)
        {
            try
            {
                var artigo = await _httpClient.GetFromJsonAsync<ArtigoModel>($"{_baseUrl}/{id}");

                // Retorna null se o artigo estiver inativo
                if (artigo != null && !artigo.Publicado)
                {
                    System.Diagnostics.Debug.WriteLine($"[ArtigoApi] Artigo {id} está inativo, bloqueado.");
                    return null;
                }

                return artigo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ArtigoApi] ERRO ao buscar artigo: {ex.Message}");
                return null;
            }
        }
    }
}
