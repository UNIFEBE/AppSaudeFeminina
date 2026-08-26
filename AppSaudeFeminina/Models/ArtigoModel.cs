namespace AppSaudeFeminina.Models
{
    public class ArtigoModel
    {
        public string Id { get; set; } = string.Empty;
        public string CategoriaId { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Resumo { get; set; } = string.Empty;
        public string? Conteudo { get; set; }
        public bool Publicado { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
