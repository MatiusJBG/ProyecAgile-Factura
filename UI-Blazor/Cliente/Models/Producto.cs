namespace UI_Blazor.Client.Models
{
    public class Producto
    {
        public int Id_Pro { get; set; }
        public string Tip_Pro { get; set; } = string.Empty;
        public string Nom_Pro { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string? Imagen { get; set; }
        
        // Para display en UI (poblado por backend desde suma de lotes)
        public int StockTotal { get; set; } = 0;
        public int NumLotes { get; set; } = 0;
        public decimal Precio_Venta { get; set; } = 0;
        public List<Lote> Lotes { get; set; } = new();

        // Propiedades para creación de lote inicial (opcional)
        public DateTime? Fec_Ent { get; set; }
        public DateTime? Fec_Exp { get; set; }
        public int? Cantidad_Recibida { get; set; }
        public int? Cantidad_Disponible { get; set; }
        public decimal? Precio_Unitario { get; set; } // Costo del lote
        
        // Propiedad auxiliar para el lote
        public decimal Precio_Lote { get; set; }
    }
}