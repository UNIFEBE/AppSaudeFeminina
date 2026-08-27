namespace AppSaudeFeminina.Models
{
    public class CicloConfigModel
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataUltimaMenstruacao { get; set; }
        public int DuracaoCiclo { get; set; } = 28;
        public int DuracaoMenstruacao { get; set; } = 5;
        public bool CicloRegular { get; set; } = true;
        public DateTime? UpdatedAt { get; set; }

        // Campos calculados retornados pela API
        public DateTime ProximoCiclo { get; set; }
        public DateTime InicioJanelaFertil { get; set; }
        public int DiasParaProximoCiclo { get; set; }
    }

    public class CreateCicloConfigRequest
    {
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataUltimaMenstruacao { get; set; }
        public int DuracaoCiclo { get; set; } = 28;
        public int DuracaoMenstruacao { get; set; } = 5;
        public bool CicloRegular { get; set; } = true;
    }

    public class RegistroDiarioModel
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; }
        public string? Humor { get; set; }
        public string Fluxo { get; set; } = "Nenhum";
        public decimal? Temperatura { get; set; }
        public string[]? Sintomas { get; set; }
        public string? Observacoes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRegistroDiarioRequest
    {
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; }
        public string? Humor { get; set; }
        public string Fluxo { get; set; } = "Nenhum";
        public decimal? Temperatura { get; set; }
        public string[]? Sintomas { get; set; }
        public string? Observacoes { get; set; }
    }

    // ── Registro de Ciclo (Histórico de Períodos) ──────────────────

    public class CicloRegistroModel
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int? DuracaoPeriodo { get; set; }
        public int? DuracaoCiclo { get; set; }
        public string? Observacoes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCicloRegistroRequest
    {
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? Observacoes { get; set; }
    }

    public class PrevisaoCicloModel
    {
        public DateTime? ProximaMenstruacao { get; set; }
        public int DiasParaProxima { get; set; }
        public int CicloMedio { get; set; }
        public int DuracaoMediaPeriodo { get; set; }
        public int TotalRegistros { get; set; }
        public List<CicloRegistroModel> UltimosRegistros { get; set; } = new();
    }
}
