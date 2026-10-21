using System.Net.Http.Json;
using System.Text.Json;
using AppSaudeFeminina.Models;

namespace AppSaudeFeminina.Services
{
    public class ConsultaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ConsultaApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _httpClient = new HttpClient(handler);

            _baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5026/api/consultas"
                : "https://localhost:7252/api/consultas";
        }

        public async Task<List<ConsultaModel>> GetByUsuarioAsync(string usuarioId)
        {
            try
            {
                var url = $"{_baseUrl}/usuario/{usuarioId}";
                System.Diagnostics.Debug.WriteLine($"[ConsultaApi] GET {url}");
                var lista = await _httpClient.GetFromJsonAsync<List<ConsultaModel>>(url, _jsonOptions);
                return lista ?? new List<ConsultaModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConsultaApi] Erro ao buscar: {ex.Message}");
                return new List<ConsultaModel>();
            }
        }

        public async Task<ConsultaModel?> CreateAsync(CreateConsultaRequest request)
        {
            try
            {
                var url = _baseUrl;
                System.Diagnostics.Debug.WriteLine($"[ConsultaApi] POST {url}");
                var response = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ConsultaApi] Erro ao criar: {err}");
                    return null;
                }
                return await response.Content.ReadFromJsonAsync<ConsultaModel>(_jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConsultaApi] Erro ao criar: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConsultaApi] Erro ao deletar: {ex.Message}");
                return false;
            }
        }
    }
}
