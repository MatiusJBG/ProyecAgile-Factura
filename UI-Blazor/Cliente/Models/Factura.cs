namespace UI_Blazor.Client.Models
{
    public enum EstadoFactura
    {
        Pendiente, Pagada, Vencida, Anulada, NotaCredito
    }

    public class Factura
    {
        public int Id { get; set; }
        public string Serie { get; set; } = "F001";
        public string Numero { get; set; } = "";
        public string NumeroCompleto => $"{Serie}-{Numero.PadLeft(8, '0')}";

        public DateTime FechaEmision { get; set; } = DateTime.Today;
        public DateTime FechaVencimiento { get; set; } = DateTime.Today.AddDays(30);

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public string Establecimiento { get; set; } = "001 - CAMINO";
        public string PuntoEmision { get; set; } = "001";

        public string Moneda { get; set; } = "USD";
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; } = 12m;
        public decimal Total { get; set; }

        public EstadoFactura Estado { get; set; } = EstadoFactura.Pendiente;
        public string Observaciones { get; set; } = "";

        public List<FacturaLinea> Lineas { get; set; } = new();
        public List<FormaPago> FormasPago { get; set; } = new();

        public bool IsSelected { get; set; } = false;
    }

    public class FacturaLinea
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; } = 0;
        public decimal Subtotal => Cantidad * PrecioUnitario - Descuento;
    }

    public class FormaPago
    {
        public string Tipo { get; set; } = "SIN UTILIZACION DE MEDIO";
        public decimal Valor { get; set; }
    }
}