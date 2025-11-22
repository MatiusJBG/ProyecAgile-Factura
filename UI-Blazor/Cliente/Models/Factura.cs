namespace UI_Blazor.Client.Models
{
    public enum EstadoFactura
    {
        Pendiente,
        Pagada,
        Vencida,
        Anulada,
        NotaCredito
    }

    public enum Establecimiento
    {
        SucursalCentro,
        SucursalNorte,
        SucursalSur,
        Matriz
    }

    public enum FormaPago
    {
        Efectivo,
        Tarjeta
    }

    public class Factura
    {
        public int Id_Fac { get; set; }
        public DateTime Fec_Fac { get; set; } = DateTime.Today;
        public int Id_Cli_Per { get; set; }
        public decimal Tot_Fac_Sin_IVA { get; set; }
        public decimal IVA_Fac { get; set; }
        public decimal Tot_Fac_Con_IVA { get; set; }

        // Estado (para UI, no en BD)
        public EstadoFactura Estado { get; set; } = EstadoFactura.Pendiente;

        // Para display en UI (poblado por backend)
        public string ClienteNombre { get; set; } = string.Empty;

        // Detalles de factura (lista de DTO)
        public List<DetalleFactura> Detalles { get; set; } = new();

        // ========== CAMPOS FUTUROS (Pendientes en BD) ==========
        
        // Establecimiento y punto de venta
        public Establecimiento Establecimiento_Fac { get; set; } = Establecimiento.Matriz;
        public string Punto_Venta { get; set; } = string.Empty;
        
        // Forma de pago
        public FormaPago Forma_Pago { get; set; } = FormaPago.Efectivo;
        
        // Valor y plazo de pago
        public decimal Valor_Pago { get; set; }
        public int Plazo_Pago_Dias { get; set; }
    }

    public class DetalleFactura
    {
        public int Id_Det_Fac { get; set; }
        public int Id_Fac_Per { get; set; }
        public int Id_Lote_Per { get; set; }
        public int Id_Pro_Per { get; set; }
        public int Cantidad_Comprada { get; set; }
        public decimal Precio_Venta_Unit { get; set; }
        public decimal Precio_Venta_Total => Cantidad_Comprada * Precio_Venta_Unit;

        // Para display (poblado por backend)
        public string ProductoNombre { get; set; } = string.Empty;
    }
}