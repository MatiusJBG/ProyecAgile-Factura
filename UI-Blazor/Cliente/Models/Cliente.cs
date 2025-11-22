namespace UI_Blazor.Client.Models
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
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // Propiedad calculada para UI
        public string NombreCompleto => Tipo_Cliente == TipoCliente.EMPRESA 
            ? Nombre 
            : $"{Nombre} {Apellido}".Trim();
    }
}