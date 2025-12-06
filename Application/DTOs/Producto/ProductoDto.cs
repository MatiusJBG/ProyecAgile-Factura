namespace Application.DTOs.Producto
{
    public class ProductoDto
    {
        public int Id_Pro { get; set; }
        public string Tip_Pro { get; set; } = string.Empty;
        public string Nom_Pro { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string? Imagen { get; set; }
        
        // Campos calculados
        public int StockTotal { get; set; }
        public int NumLotes { get; set; }
        
        // Información adicional
        public decimal Precio_Venta { get; set; }
        public List<LoteDto> Lotes { get; set; } = new();

        // Campos para edición de lote (opcionales)
        public DateTime? Fec_Ent { get; set; }
        public DateTime? Fec_Exp { get; set; }
        public int? Cantidad_Recibida { get; set; }
        public int? Cantidad_Disponible { get; set; }
        public decimal? Precio_Unitario { get; set; }
        
        // Auxiliar
        public decimal Precio_Lote { get; set; }
    }
}
