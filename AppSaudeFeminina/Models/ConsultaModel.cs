namespace AppSaudeFeminina.Models
{
    public class ConsultaModel
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;
        public DateTime DataConsulta { get; set; }
        public string Status { get; set; } = "Confirmada";
        public DateTime CreatedAt { get; set; }
    }

    public class CreateConsultaRequest
    {
        public Guid UsuarioId { get; set; }
        public string Especialidade { get; set; } = string.Empty;
        public DateTime DataConsulta { get; set; }
        public string Status { get; set; } = "Confirmada";
    }
}
