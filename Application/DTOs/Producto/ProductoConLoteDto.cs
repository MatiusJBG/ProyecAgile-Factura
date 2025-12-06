namespace Application.DTOs.Producto
{
    public class ProductoConLoteDto
    {
        // Información del producto
        public string Tip_Pro { get; set; } = string.Empty;
        public string Nom_Pro { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string? Imagen { get; set; }

        // Información del lote (opcional)
        public DateTime? Fec_Ent { get; set; }
        public DateTime? Fec_Exp { get; set; }
        public int? Cantidad_Recibida { get; set; }
        public int? Cantidad_Disponible { get; set; }

        public decimal? Precio_Unitario { get; set; } // Costo del lote
        public decimal? Precio_Venta { get; set; } // Precio de venta al público
    }
}
