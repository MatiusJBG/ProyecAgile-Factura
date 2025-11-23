namespace Application.DTOs
{
    public class ProductoDto
    {
        public int Id_Pro { get; set; }
        public string Tip_Pro { get; set; } = string.Empty;
        public string Nom_Pro { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        
        // Campos calculados
        public int StockTotal { get; set; }
        public int NumLotes { get; set; }
    }
}
