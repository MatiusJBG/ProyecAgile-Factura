using Core.Entities.Clientes;

namespace Core.Entities.Facturacion
{
    public class Factura
    {
        public int Id_Fac { get; set; }
        public DateTime Fec_Fac { get; set; }
        public int Id_Cli_Per { get; set; }
        public decimal? Tot_Fac_Sin_IVA { get; set; }
        public decimal? IVA_Fac { get; set; }
        public decimal? Tot_Fac_Con_IVA { get; set; }
        
        // Estado de la factura
        public Core.Enums.Facturacion.EstadoFactura Estado { get; set; } = Core.Enums.Facturacion.EstadoFactura.NoEnviada;

        // Clave de Acceso generada
        public string? ClaveAcceso { get; set; }

        public string? MensajeError { get; set; } // Para guardar mensajes de error del SRI

        // Navigation properties
        public Cliente Cliente { get; set; } = null!;
        public ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    }
}

