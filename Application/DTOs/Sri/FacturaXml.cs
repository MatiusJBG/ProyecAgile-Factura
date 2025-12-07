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
        public InfoTributariaXml InfoTributaria { get; set; }

        [XmlElement("infoFactura")]
        public InfoFacturaXml InfoFactura { get; set; }

        [XmlArray("detalles")]
        [XmlArrayItem("detalle")]
        public List<DetalleFacturaXml> Detalles { get; set; }
    }

    public class InfoTributariaXml
    {
        public string ambiente { get; set; }
        public string tipoEmision { get; set; }
        public string razonSocial { get; set; }
        public string nombreComercial { get; set; }
        public string ruc { get; set; }
        public string claveAcceso { get; set; }
        public string codDoc { get; set; }
        public string estab { get; set; }
        public string ptoEmi { get; set; }
        public string secuencial { get; set; }
        public string dirMatriz { get; set; }
    }

    public class InfoFacturaXml
    {
        public string fechaEmision { get; set; }
        public string dirEstablecimiento { get; set; }
        // public string contribuyenteEspecial { get; set; } // Opcional
        public string obligadoContabilidad { get; set; } = "NO"; // Default or mapped
        public string tipoIdentificacionComprador { get; set; }
        public string razonSocialComprador { get; set; }
        public string identificacionComprador { get; set; }
        public string totalSinImpuestos { get; set; }
        public string totalDescuento { get; set; }

        [XmlArray("totalConImpuestos")]
        [XmlArrayItem("totalImpuesto")]
        public List<TotalImpuestoXml> TotalConImpuestos { get; set; }

        public string propina { get; set; }
        public string importeTotal { get; set; }
        public string moneda { get; set; }
    }

    public class TotalImpuestoXml
    {
        public string codigo { get; set; }
        public string codigoPorcentaje { get; set; }
        public string baseImponible { get; set; }
        public string valor { get; set; }
        // tarifa is optional in TotalImpuesto? It is present in DetalleImpuesto.
        // Checking user snippet: user sets baseImponible and valor. 
        // Some XSDs require tarifa here too, but user snippet commented it out. I'll add it as optional property just in case.
        // tarifa is NOT allowed in TotalImpuesto in standard SRI XSD 1.0.0/1.1.0
        // [XmlElement(IsNullable = false)]
        // public string tarifa { get; set; }  
    }

    public class DetalleFacturaXml
    {
        public string codigoPrincipal { get; set; }
        public string descripcion { get; set; }
        public decimal cantidad { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal descuento { get; set; }
        public decimal precioTotalSinImpuesto { get; set; }

        [XmlArray("impuestos")]
        [XmlArrayItem("impuesto")]
        public List<ImpuestoDetalleXml> impuesto { get; set; }
    }

    public class ImpuestoDetalleXml
    {
        public string codigo { get; set; }
        public string codigoPorcentaje { get; set; }
        public decimal tarifa { get; set; }
        public decimal baseImponible { get; set; }
        public decimal valor { get; set; }
    }
}
