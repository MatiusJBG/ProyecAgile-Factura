using Application.DTOs.Sri;
using Core.Entities.Facturacion;
using Core.Enums.Clientes;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Infrastructure.Services.Facturacion
{
    public class FacturaXmlService
    {
        private readonly IConfiguration _configuration;

        public FacturaXmlService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FacturaXml MapToXmlModel(Factura factura)
        {
            var ci = CultureInfo.InvariantCulture;

            // Constantes para pruebas (15% IVA)
            const decimal IVA_RATE_PARA_PRUEBAS = 0.15M; 
            const string CODIGO_PORCENTAJE_PARA_PRUEBAS = "4"; 

            // Obtener datos del emisor desde configuración
            var rucEmisor = _configuration["Sri:RucEmisor"] ?? "1804368858001";
            var razonSocialEmisor = _configuration["Sri:RazonSocialEmisor"] ?? "PRUEBAS SERVICIO DE RENTAS INTERNAS";
            var direccionMatriz = _configuration["Sri:DireccionMatriz"] ?? "QUITO";
            var contribuyenteEspecial = _configuration["Sri:ContribuyenteEspecial"];
            var obligadoContabilidad = _configuration["Sri:ObligadoContabilidad"] ?? "NO";

            // Generar secuencial y serie (asumiendo que vienen formateados o calculados previamente, si no, usar defaults)
            // En un caso real, esto vendría de un PuntoEmision configurado.
            // Por simplicidad, usaremos valores fijos o extraídos si existieran en Factura (no existen en la entidad actual).
            var estab = "001";
            var ptoEmi = "001";
            var secuencial = factura.Id_Fac.ToString().PadLeft(9, '0'); // Usar ID factura como secuencial por ahora

            var rnd = new Random();
            var codigoNumerico = rnd.Next(10000000, 99999999).ToString();
            var claveAcceso = GenerarClaveAcceso(factura.Fec_Fac, "01", rucEmisor, "1", estab + ptoEmi, secuencial, codigoNumerico, "1");

            var infoTrib = new InfoTributariaXml
            {
                ambiente = "1", // 1: Pruebas, 2: Producción
                tipoEmision = "1", // 1: Normal
                razonSocial = razonSocialEmisor,
                nombreComercial = razonSocialEmisor,
                ruc = rucEmisor,
                claveAcceso = claveAcceso,
                codDoc = "01", // Factura
                estab = estab,
                ptoEmi = ptoEmi,
                secuencial = secuencial,
                dirMatriz = direccionMatriz
            };

            var totalConImpuestos = new List<TotalImpuestoXml>();
            if (factura.IVA_Fac > 0)
            {
                // Asumiendo que todo el IVA es del 15% para pruebas
                totalConImpuestos.Add(new TotalImpuestoXml
                {
                    codigo = "2", // IVA
                    codigoPorcentaje = CODIGO_PORCENTAJE_PARA_PRUEBAS,
                    baseImponible = (factura.Tot_Fac_Sin_IVA ?? 0m).ToString("0.00", ci),
                    valor = (factura.IVA_Fac ?? 0m).ToString("0.00", ci)
                });
            }
            else
            {
                 totalConImpuestos.Add(new TotalImpuestoXml
                {
                    codigo = "2", // IVA
                    codigoPorcentaje = "0", // 0%
                    baseImponible = (factura.Tot_Fac_Sin_IVA ?? 0m).ToString("0.00", ci),
                    valor = "0.00"
                });
            }

            var infoFac = new InfoFacturaXml
            {
                fechaEmision = factura.Fec_Fac.ToString("dd/MM/yyyy"),
                dirEstablecimiento = direccionMatriz,
                tipoIdentificacionComprador = ObtenerCodigoTipoIdentificacion(factura.Cliente?.Tipo_Documento),
                razonSocialComprador = factura.Cliente != null ? $"{factura.Cliente.Nombre} {factura.Cliente.Apellido}".Trim() : "CONSUMIDOR FINAL",
                identificacionComprador = factura.Cliente?.Num_Documento ?? "9999999999999",
                totalSinImpuestos = (factura.Tot_Fac_Sin_IVA ?? 0m).ToString("0.00", ci),
                totalDescuento = "0.00", // No tenemos campo descuento en cabecera explícito, asumir 0 o sumar detalles
                TotalConImpuestos = totalConImpuestos,
                propina = "0.00",
                importeTotal = (factura.Tot_Fac_Con_IVA ?? 0m).ToString("0.00", ci),
                moneda = "USD",
                obligadoContabilidad = obligadoContabilidad
            };

            var detalles = factura.Detalles.Select(d =>
            {
                // Calcular base imponible e IVA por detalle (aproximación si no está guardado)
                // Asumimos que Precio_Venta_Unit ya incluye o no IVA? El modelo dice Tot_Fac_Sin_IVA...
                // Detalle tiene Precio_Venta_Total. Asumiremos que es Sin IMPUESTOS o tenemos que calcularlo.
                // DetalleFactura.cs: Precio_Venta_Unit, Cantidad_Comprada, Porcentaje_Descuento, Precio_Venta_Total.
                
                // Lógica simplificada: todo al 15% si la factura tiene IVA, o mixta?
                // Por ahora, asumimos que si la factura tiene IVA, usamos código 4.
                
                var impuesto = new ImpuestoDetalleXml
                {
                    codigo = "2",
                    codigoPorcentaje = factura.IVA_Fac > 0 ? CODIGO_PORCENTAJE_PARA_PRUEBAS : "0",
                    tarifa = factura.IVA_Fac > 0 ? 15.00M : 0.00M,
                    baseImponible = Math.Round((decimal)d.Precio_Venta_Total, 2),
                    valor = Math.Round(factura.IVA_Fac > 0 ? (decimal)d.Precio_Venta_Total * IVA_RATE_PARA_PRUEBAS : 0, 2)
                };

                return new DetalleFacturaXml
                {
                    codigoPrincipal = d.Id_Pro_Per.ToString(),
                    descripcion = "Producto " + d.Id_Pro_Per, 
                    cantidad = Math.Round((decimal)d.Cantidad_Comprada, 6), // Allow up to 6 decimals for quantity
                    precioUnitario = Math.Round((decimal)d.Precio_Venta_Unit, 6), // Allow up to 6 decimals for unit price
                    descuento = Math.Round((decimal)((d.Precio_Venta_Unit * d.Cantidad_Comprada) * (d.Porcentaje_Descuento / 100)), 2),
                    precioTotalSinImpuesto = Math.Round((decimal)d.Precio_Venta_Total, 2),
                    impuesto = new List<ImpuestoDetalleXml> { impuesto }
                };
            }).ToList();

            return new FacturaXml
            {
                InfoTributaria = infoTrib,
                InfoFactura = infoFac,
                Detalles = detalles
            };
        }

        private string ObtenerCodigoTipoIdentificacion(TipoDocumento? tipo)
        {
            if (tipo == null) return "07"; // Consumidor Final
            return tipo switch
            {
                TipoDocumento.RUC => "04",
                TipoDocumento.CEDULA => "05",
                TipoDocumento.PASAPORTE => "06",
                _ => "07"
            };
        }

        public string GenerarClaveAcceso(DateTime fechaEmision, string codDoc, string ruc, string ambiente, string serie, string secuencial, string codigoNumerico, string tipoEmision)
        {
            var fecha = fechaEmision.ToString("ddMMyyyy");
            var baseClave = fecha + codDoc + ruc + ambiente + serie + secuencial + codigoNumerico + tipoEmision;
            var digito = CalcularDigitoVerificador(baseClave);
            return baseClave + digito;
        }

        private int CalcularDigitoVerificador(string claveSinDigito)
        {
            int[] pesos = { 2, 3, 4, 5, 6, 7 };
            int suma = 0;
            int pesoIndex = 0;
            for (int i = claveSinDigito.Length - 1; i >= 0; i--)
            {
                int digito = claveSinDigito[i] - '0';
                suma += digito * pesos[pesoIndex];
                pesoIndex++;
                if (pesoIndex >= pesos.Length)
                {
                    pesoIndex = 0;
                }
            }
            int modulo = suma % 11;
            int digitoVerificador = 11 - modulo;
            if (digitoVerificador == 11) { return 0; }
            if (digitoVerificador == 10) { return 1; }
            return digitoVerificador;
        }

        public FacturaXmlResult GenerarXmlResult(Factura factura)
        {
            var facturaXml = MapToXmlModel(factura);
            var claveAcceso = facturaXml.InfoTributaria.claveAcceso;
            
            // Fix: remove namespaces to keep it clean or add SRI namespaces as required
            var ns = new XmlSerializerNamespaces();
            ns.Add("", ""); 

            var serializer = new XmlSerializer(typeof(FacturaXml));
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false), // Sin BOM
                Indent = true,
                OmitXmlDeclaration = false
            };
            
            using var ms = new MemoryStream();
            using (var writer = XmlWriter.Create(ms, settings))
            {
                serializer.Serialize(writer, facturaXml, ns);
            }
            
            return new FacturaXmlResult 
            { 
                XmlBytes = ms.ToArray(),
                ClaveAcceso = claveAcceso
            };
        }

        // Maintain backward compatibility if needed, or just redirect
        public byte[] GenerarXmlBytes(Factura factura) => GenerarXmlResult(factura).XmlBytes;
    }
}
