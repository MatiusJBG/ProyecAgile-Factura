using Core.Enums.Clientes; using Core.Enums.Facturacion;

namespace Application.DTOs.Factura
{
    public class FacturaDto
    {
        public int Id_Fac { get; set; }
        public DateTime Fec_Fac { get; set; } = DateTime.Today;
        public int Id_Cli_Per { get; set; }
        
        public decimal Tot_Fac_Sin_IVA { get; set; }
        public decimal Tot_Descuento { get; set; } 
        public decimal IVA_Fac { get; set; }
        public decimal Tot_Fac_Con_IVA { get; set; }
        
        // Estado
        public EstadoFactura Estado { get; set; } = EstadoFactura.Pendiente;
        
        // Para display en UI
        public string ClienteNombre { get; set; } = string.Empty;
        
        // Detalles de factura
        public List<DetalleFacturaDto> Detalles { get; set; } = new();

        // ========== Campos desde UI Model ==========
        
        // Forma de pago
        public FormaPago Forma_Pago { get; set; } = FormaPago.Efectivo;
        
        // Valor de pago
        public decimal Valor_Pago { get; set; }

        // Datos de Tarjeta (UI)
        public string NumeroTarjeta { get; set; } = string.Empty;
        public string TitularTarjeta { get; set; } = string.Empty;
        public string FechaVencimiento { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
    }

    public class DetalleFacturaDto
    {
        public int Id_Det_Fac { get; set; }
        public int Id_Fac_Per { get; set; }
        public int Id_Lote_Per { get; set; }
        public int Id_Pro_Per { get; set; }
        public int Cantidad_Comprada { get; set; }
        public decimal Precio_Venta_Unit { get; set; }
        
        public decimal Porcentaje_Descuento { get; set; }
        
        // Propiedades calculadas
        public decimal Monto_Descuento => (Cantidad_Comprada * Precio_Venta_Unit) * (Porcentaje_Descuento / 100m);
        public decimal Precio_Venta_Total => (Cantidad_Comprada * Precio_Venta_Unit) - Monto_Descuento;
        
        // Para display
        public string ProductoNombre { get; set; } = string.Empty;
    }
}