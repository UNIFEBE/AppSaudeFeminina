using System.Net.Http.Json;
using System.Text.Json;
using AppSaudeFeminina.Models;

namespace AppSaudeFeminina.Services
{
    public class CicloApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CicloApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _httpClient = new HttpClient(handler);

            if (DeviceInfo.Platform == DevicePlatform.Android)
                _baseUrl = "http://10.0.2.2:5026/api/ciclo";
            else
                _baseUrl = "https://localhost:7252/api/ciclo";
        }

        // ── Configuração do Ciclo ──────────────────────────────────────

        public async Task<CicloConfigModel?> GetConfigAsync(string usuarioId)
        {
            try
            {
                var url = $"{_baseUrl}/config/{usuarioId}";
                System.Diagnostics.Debug.WriteLine($"[CicloApi] GET {url}");

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[CicloApi] Config não encontrada: {response.StatusCode}");
                    return null;
                }

                var config = await response.Content.ReadFromJsonAsync<CicloConfigModel>(_jsonOptions);
                System.Diagnostics.Debug.WriteLine($"[CicloApi] Config carregada: ciclo={config?.DuracaoCiclo}, fluxo={config?.DuracaoMenstruacao}");
                return config;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao buscar config: {ex.Message}");
                return null;
            }
        }

        public async Task<CicloConfigModel?> SaveConfigAsync(CreateCicloConfigRequest request)
        {
            try
            {
                var url = $"{_baseUrl}/config";
                System.Diagnostics.Debug.WriteLine($"[CicloApi] POST {url}");

                var response = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[CicloApi] Erro ao salvar config: {error}");
                    return null;
                }

                var config = await response.Content.ReadFromJsonAsync<CicloConfigModel>(_jsonOptions);
                System.Diagnostics.Debug.WriteLine($"[CicloApi] Config salva com sucesso");
                return config;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao salvar config: {ex.Message}");
                return null;
            }
        }

        // ── Registros Diários ──────────────────────────────────────────

        public async Task<List<RegistroDiarioModel>> GetRegistrosAsync(string usuarioId)
        {
            try
            {
                var url = $"{_baseUrl}/registros/{usuarioId}";
                System.Diagnostics.Debug.WriteLine($"[CicloApi] GET {url}");

                var registros = await _httpClient.GetFromJsonAsync<List<RegistroDiarioModel>>(url, _jsonOptions);
                System.Diagnostics.Debug.WriteLine($"[CicloApi] Retornou {registros?.Count ?? 0} registros");
                return registros ?? new List<RegistroDiarioModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao buscar registros: {ex.Message}");
                return new List<RegistroDiarioModel>();
            }
        }

        public async Task<List<RegistroDiarioModel>> GetRegistrosByPeriodoAsync(string usuarioId, DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                var url = $"{_baseUrl}/registros/{usuarioId}/periodo?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}";
                System.Diagnostics.Debug.WriteLine($"[CicloApi] GET {url}");

                var registros = await _httpClient.GetFromJsonAsync<List<RegistroDiarioModel>>(url, _jsonOptions);
                return registros ?? new List<RegistroDiarioModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao buscar registros por período: {ex.Message}");
                return new List<RegistroDiarioModel>();
            }
        }

        public async Task<RegistroDiarioModel?> SaveRegistroAsync(CreateRegistroDiarioRequest request)
        {
            try
            {
                var url = $"{_baseUrl}/registros";
                System.Diagnostics.Debug.WriteLine($"[CicloApi] POST {url}");

                var response = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[CicloApi] Erro ao salvar registro: {error}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<RegistroDiarioModel>(_jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao salvar registro: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteRegistroAsync(string id)
        {
            try
            {
                var url = $"{_baseUrl}/registros/{id}";
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao deletar registro: {ex.Message}");
                return false;
            }
        }

        // ── Registros de Ciclo (Histórico de Períodos) ─────────────────

        private string RegistrosBaseUrl => _baseUrl.Replace("/api/ciclo", "/api/ciclo-registros");

        public async Task<PrevisaoCicloModel?> GetPrevisaoAsync(string usuarioId)
        {
            try
            {
                var url = $"{RegistrosBaseUrl}/{usuarioId}/previsao";
                System.Diagnostics.Debug.WriteLine($"[CicloApi] GET {url}");

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<PrevisaoCicloModel>(_jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao buscar previsão: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CicloRegistroModel>> GetCicloRegistrosAsync(string usuarioId)
        {
            try
            {
                var url = $"{RegistrosBaseUrl}/{usuarioId}";
                System.Diagnostics.Debug.WriteLine($"[CicloApi] GET {url}");

                var registros = await _httpClient.GetFromJsonAsync<List<CicloRegistroModel>>(url, _jsonOptions);
                return registros ?? new List<CicloRegistroModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao buscar ciclo registros: {ex.Message}");
                return new List<CicloRegistroModel>();
            }
        }

        public async Task<CicloRegistroModel?> CreateCicloRegistroAsync(CreateCicloRegistroRequest request)
        {
            try
            {
                var url = RegistrosBaseUrl;
                System.Diagnostics.Debug.WriteLine($"[CicloApi] POST {url}");

                var response = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[CicloApi] Erro ao criar ciclo registro: {error}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<CicloRegistroModel>(_jsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao criar ciclo registro: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteCicloRegistroAsync(string id)
        {
            try
            {
                var url = $"{RegistrosBaseUrl}/{id}";
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CicloApi] ERRO ao deletar ciclo registro: {ex.Message}");
                return false;
            }
        }
    }
}
