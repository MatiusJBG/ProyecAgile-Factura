namespace Core.Entities
{
    public class FirmaElectronica
    {
        public int Id_Firma { get; set; }
        public int Id_Fac_Per { get; set; }
        public string Firma_Digital { get; set; } = string.Empty; // Base64
        public string Algoritmo { get; set; } = "SHA256withRSA";
        public string Certificado_Serial { get; set; } = string.Empty;
        public DateTime Fecha_Firma { get; set; } = DateTime.UtcNow;
        public string Hash_Documento { get; set; } = string.Empty;
        public string Estado_Validacion { get; set; } = "Pendiente"; // Pendiente, Valida, Invalida
        public string? Observaciones { get; set; }

        // Navigation property
        public Factura? Factura { get; set; }
    }
}
