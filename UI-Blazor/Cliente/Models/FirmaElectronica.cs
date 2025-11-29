namespace UI_Blazor.Client.Models
{
    public class FirmaElectronica
    {
        public int Id_Firma { get; set; }
        public int Id_Fac_Per { get; set; }
        public string Firma_Digital { get; set; } = string.Empty;
        public string Algoritmo { get; set; } = string.Empty;
        public string Certificado_Serial { get; set; } = string.Empty;
        public DateTime Fecha_Firma { get; set; }
        public string Hash_Documento { get; set; } = string.Empty;
        public string Estado_Validacion { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
