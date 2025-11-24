namespace Application.DTOs
{
    public class FacturaDto
    {
        public int Id_Fac { get; set; }
        public DateTime Fec_Fac { get; set; }
        public int Id_Cli_Per { get; set; }
        
        // AGREGADO: Total de Descuento (para consistencia en la capa de aplicación)
        public decimal Tot_Descuento { get; set; } 
        
        public decimal Tot_Fac_Sin_IVA { get; set; }
        public decimal IVA_Fac { get; set; }
        public decimal Tot_Fac_Con_IVA { get; set; }
        
        // Para display en UI
        public string ClienteNombre { get; set; } = string.Empty;
        
        // Detalles de factura
        public List<DetalleFacturaDto> Detalles { get; set; } = new();
    }

    public class DetalleFacturaDto
    {
        public int Id_Det_Fac { get; set; }
        public int Id_Fac_Per { get; set; }
        public int Id_Lote_Per { get; set; }
        public int Id_Pro_Per { get; set; }
        public int Cantidad_Comprada { get; set; }
        public decimal Precio_Venta_Unit { get; set; }
        
        // AGREGADO: Propiedad faltante que causaba el error CS1061
        public decimal Porcentaje_Descuento { get; set; }
        
        public decimal Precio_Venta_Total { get; set; }
        
        // Para display
        public string ProductoNombre { get; set; } = string.Empty;
    }
}