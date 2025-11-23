namespace Core.Entities
{
    public enum TipoCliente
    {
        PERSONA,
        EMPRESA,
        EXTRANJERO
    }

    public enum TipoDocumento
    {
        CEDULA,
        RUC,
        PASAPORTE
    }

    public class Cliente
    {
        public int Id_Cli { get; set; }
        public TipoCliente Tipo_Cliente { get; set; }
        public TipoDocumento Tipo_Documento { get; set; }
        public string Num_Documento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Apellido { get; set; }
        public string? Direccion { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }

        // Navigation properties
        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}
