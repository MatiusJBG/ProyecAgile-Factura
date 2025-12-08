namespace Infrastructure.Services.Facturacion
{
    public class FacturaXmlResult
    {
        public byte[] XmlBytes { get; set; } = Array.Empty<byte>();
        public string ClaveAcceso { get; set; } = string.Empty;
    }
}
