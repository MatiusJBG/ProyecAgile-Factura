namespace Core.Entities.Certificados
{
    public class CertificadoDigital
    {
        public int Id_Cert { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ruta_Archivo { get; set; } = string.Empty; // Path relativo al archivo .p12/.pfx
        public string Password_Hash { get; set; } = string.Empty; // Hash de la contraseña (no guardar en texto plano)
        public DateTime Fecha_Emision { get; set; }
        public DateTime Fecha_Expiracion { get; set; }
        public string Emisor { get; set; } = string.Empty; // CA que emitió el certificado
        public string Serial_Number { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty; // DN del titular
        public bool Activo { get; set; } = false;
        public DateTime Fecha_Carga { get; set; } = DateTime.UtcNow;
        public string? Observaciones { get; set; }
    }
}

