namespace UI_Blazor.Client.Models
{

    public enum TipoCliente
    {
        PersonaNatural,
        Empresa,
        Extranjero
    }

    public class Cliente
    {
        public int Id { get; set; }

        //Basic Information
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos {get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;

        //Identification
        public TipoCliente Tipo { get; set; }
        public string NumeroIdentificacion { get; set; } = string.Empty;

        //Contact Information and Status
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime UltimaFactura { get; set; }
        public decimal Saldo { get; set; }
        public string Estado { get; set; } = string.Empty;

        public string NombreCompleto => Tipo == TipoCliente.Empresa ? NombreComercial : $"{Nombres} {Apellidos}";
        public string TipoDocumento => Tipo switch
        {
            TipoCliente.PersonaNatural => "Cédula",
            TipoCliente.Empresa => "RUC",
            TipoCliente.Extranjero => "Pasaporte",
            _ => "Desconocido"
        };
        public bool IsSelected { get; set; } = false;
    }
}