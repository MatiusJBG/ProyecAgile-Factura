using System.Xml.Serialization;

namespace Application.DTOs.Sri
{
    [XmlRoot("factura")]
    public class FacturaXml
    {
        [XmlAttribute("id")]
        public string id { get; set; } = "comprobante";

        [XmlAttribute("version")]
        public string version { get; set; } = "1.0.0";

        [XmlElement("infoTributaria")]
        public InfoTributariaXml InfoTributaria { get; set; } = new();

        [XmlElement("infoFactura")]
        public InfoFacturaXml InfoFactura { get; set; } = new();

        [XmlArray("detalles")]
        [XmlArrayItem("detalle")]
        public List<DetalleFacturaXml> Detalles { get; set; } = new();
    }

    public class InfoTributariaXml
    {
        public string ambiente { get; set; } = string.Empty;
        public string tipoEmision { get; set; } = string.Empty;
        public string razonSocial { get; set; } = string.Empty;
        public string nombreComercial { get; set; } = string.Empty;
        public string ruc { get; set; } = string.Empty;
        public string claveAcceso { get; set; } = string.Empty;
        public string codDoc { get; set; } = string.Empty;
        public string estab { get; set; } = string.Empty;
        public string ptoEmi { get; set; } = string.Empty;
        public string secuencial { get; set; } = string.Empty;
        public string dirMatriz { get; set; } = string.Empty;
    }

    public class InfoFacturaXml
    {
        public string fechaEmision { get; set; } = string.Empty;
        public string dirEstablecimiento { get; set; } = string.Empty;
        // public string contribuyenteEspecial { get; set; } // Opcional
        public string obligadoContabilidad { get; set; } = "NO"; 
        public string tipoIdentificacionComprador { get; set; } = string.Empty;
        public string razonSocialComprador { get; set; } = string.Empty;
        public string identificacionComprador { get; set; } = string.Empty;
        public string totalSinImpuestos { get; set; } = string.Empty;
        public string totalDescuento { get; set; } = string.Empty;

        [XmlArray("totalConImpuestos")]
        [XmlArrayItem("totalImpuesto")]
        public List<TotalImpuestoXml> TotalConImpuestos { get; set; } = new();

        public string propina { get; set; } = "0.00";
        public string importeTotal { get; set; } = string.Empty;
        public string moneda { get; set; } = "DOLAR";
    }

    public class TotalImpuestoXml
    {
        public string codigo { get; set; } = string.Empty;
        public string codigoPorcentaje { get; set; } = string.Empty;
        public string baseImponible { get; set; } = string.Empty;
        public string valor { get; set; } = string.Empty;
    }

    public class DetalleFacturaXml
    {
        public string codigoPrincipal { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public decimal cantidad { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal descuento { get; set; }
        public decimal precioTotalSinImpuesto { get; set; }

        [XmlArray("impuestos")]
        [XmlArrayItem("impuesto")]
        public List<ImpuestoDetalleXml> impuesto { get; set; } = new();
    }

    public class ImpuestoDetalleXml
    {
        public string codigo { get; set; } = string.Empty;
        public string codigoPorcentaje { get; set; } = string.Empty;
        public decimal tarifa { get; set; }
        public decimal baseImponible { get; set; }
        public decimal valor { get; set; }
    }
}
