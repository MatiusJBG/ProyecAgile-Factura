namespace UI_Blazor.Client.Models
{
    public enum TipoProducto
    {
        BIEN,
        SERVICIO
    }

    public enum TipoIVA
    {
        Gravado0,
        Gravado12,
        Gravado14,
        NoObjetoIVA,
        ExentoIVA
    }

    public enum TipoICE
    {
        SinICE,
        Con3,
        Con5,
        Con10
    }

    public enum TipoIRBPNR
    {
        SinIRBPNR,
        Con1,
        Con2,
        Con5
    }

    public class Producto
    {
        public int Id { get; set; }
        
        // Características Generales
        public string CodigoPrincipal { get; set; } = string.Empty;
        public string CodigoAuxiliar { get; set; } = string.Empty;
        public TipoProducto TipoProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
        // Información de Lote
        public string NumeroLote { get; set; } = string.Empty;
        public int CantidadEnLote { get; set; } = 1;
        public DateTime? FechaLote { get; set; }
        public decimal PrecioLote { get; set; }
        
        // Impuestos Aplicables
        public TipoIVA IVA { get; set; }
        public TipoICE ICE { get; set; }
        public TipoIRBPNR IRBPNR { get; set; }
        
        // Información Adicional (3 atributos personalizados)
        public string? Atributo1Nombre { get; set; }
        public string? Atributo1Descripcion { get; set; }
        
        public string? Atributo2Nombre { get; set; }
        public string? Atributo2Descripcion { get; set; }
        
        public string? Atributo3Nombre { get; set; }
        public string? Atributo3Descripcion { get; set; }
        
        // Control de inventario (opcional - no está en el formulario original pero útil)
        public int Stock { get; set; }
        public decimal? PrecioCompra { get; set; }
        
        // Estado
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }
        
        // Propiedades calculadas
        public string TipoProductoTexto => TipoProducto == TipoProducto.BIEN ? "BIEN" : "SERVICIO";
        public string IVATexto => IVA switch
        {
            TipoIVA.Gravado0 => "0%",
            TipoIVA.Gravado12 => "12%",
            TipoIVA.Gravado14 => "14%",
            TipoIVA.NoObjetoIVA => "No Objeto IVA",
            TipoIVA.ExentoIVA => "Exento IVA",
            _ => "N/A"
        };
        
        public bool IsSelected { get; set; } = false;
    }
}