namespace Application.DTOs.Certificados
{
    public class CertificadoDigitalDto
    {
        public int Id_Cert { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ruta_Archivo { get; set; } = string.Empty;
        public DateTime Fecha_Emision { get; set; }
        public DateTime Fecha_Expiracion { get; set; }
        public string Emisor { get; set; } = string.Empty;
        public string Serial_Number { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime Fecha_Carga { get; set; }
        public string? Observaciones { get; set; }

        // Propiedades calculadas para UI
        public bool EstaExpirado => Fecha_Expiracion < DateTime.UtcNow;
        public int DiasParaExpirar => (Fecha_Expiracion - DateTime.UtcNow).Days;
        public string EstadoTexto => Activo ? "Activo" : "Inactivo";
    }
}
