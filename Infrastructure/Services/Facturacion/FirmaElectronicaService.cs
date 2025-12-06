using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Exceptions;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace Infrastructure.Services.Facturacion
{
    public class FirmaElectronicaService : IFirmaElectronicaService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IFirmaElectronicaRepository _firmaRepository;
        private readonly ICertificadoService _certificadoService;
        private readonly IConfiguration _configuration;

        public FirmaElectronicaService(
            IFacturaRepository facturaRepository,
            IFirmaElectronicaRepository firmaRepository,
            ICertificadoService certificadoService,
            IConfiguration configuration)
        {
            _facturaRepository = facturaRepository;
            _firmaRepository = firmaRepository;
            _certificadoService = certificadoService;
            _configuration = configuration;
        }

        public async Task<FirmaElectronica> FirmarFacturaAsync(int idFactura)
        {
            Console.WriteLine($"Iniciando firma de factura ID: {idFactura}");

            // Obtener la factura con sus detalles usando el repositorio
            var factura = await _facturaRepository.GetFacturaWithDetailsAsync(idFactura);
            if (factura == null)
                throw new EntityNotFoundException("Factura", idFactura);

            Console.WriteLine("Factura encontrada.");

            // Verificar si ya está firmada usando el repositorio
            var firmaExistente = await _firmaRepository.GetByFacturaIdAsync(idFactura);
            if (firmaExistente != null)
                throw new DuplicateEntityException("La factura ya está firmada");

            // Obtener certificado activo
            var certificado = await _certificadoService.GetCertificadoActivoAsync();
            if (certificado == null)
                throw new BusinessValidationException("No hay certificado activo configurado");

            Console.WriteLine($"Certificado activo encontrado: {certificado.Nombre}");

            // Generar XML de la factura
            string xmlFactura = GenerarXmlFactura(factura);
            Console.WriteLine("XML generado correctamente.");

            // Generar hash del XML
            string hashDocumento = GenerarHash(xmlFactura);
            Console.WriteLine("Hash generado.");

            // Cargar el certificado X509 (con password opcional)
            Console.WriteLine("Cargando certificado X509...");
            var cert = await _certificadoService.CargarCertificadoX509Async(certificado.Id_Cert);
            Console.WriteLine("Certificado X509 cargado.");

            // Firmar el XML con el certificado
            Console.WriteLine("Firmando XML...");
            string firmaDigital = FirmarConCertificado(xmlFactura, cert);
            Console.WriteLine("XML firmado.");

            // Crear registro de firma
            var firma = new FirmaElectronica
            {
                Id_Fac_Per = idFactura,
                Firma_Digital = firmaDigital,
                Algoritmo = "SHA256withRSA",
                Certificado_Serial = cert.SerialNumber,
                Fecha_Firma = DateTime.UtcNow,
                Hash_Documento = hashDocumento,
                Estado_Validacion = "Valida",
                Observaciones = "Firmado correctamente"
            };

            // Guardar usando el repositorio
            await _firmaRepository.AddAsync(firma);
            Console.WriteLine("Firma guardada en base de datos.");

            return firma;
        }

        public async Task<bool> ValidarFirmaAsync(int idFactura)
        {
            // Usar repositorio para obtener la firma
            var firma = await _firmaRepository.GetByFacturaIdAsync(idFactura);
            if (firma == null)
                return false;

            // Usar repositorio para obtener la factura
            var factura = await _facturaRepository.GetFacturaWithDetailsAsync(idFactura);
            if (factura == null)
                return false;

            // Regenerar XML de la factura
            string xmlActual = GenerarXmlFactura(factura);

            // Calcular hash actual
            string hashActual = GenerarHash(xmlActual);

            // Comparar con el hash almacenado
            bool hashCoincide = hashActual == firma.Hash_Documento;

            // Actualizar estado de validación
            firma.Estado_Validacion = hashCoincide ? "Valida" : "Invalida";
            firma.Observaciones = hashCoincide 
                ? "Firma válida - documento no modificado" 
                : "Firma inválida - documento ha sido modificado";

            // Actualizar usando el repositorio
            await _firmaRepository.UpdateAsync(firma);

            return hashCoincide;
        }

        public async Task<FirmaElectronica?> GetFirmaPorFacturaAsync(int idFactura)
        {
            return await _firmaRepository.GetByFacturaIdAsync(idFactura);
        }

        public string GenerarXmlFactura(Factura factura)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Factura");

            // Información básica
            xmlWriter.WriteElementString("Id", factura.Id_Fac.ToString());
            xmlWriter.WriteElementString("Fecha", factura.Fec_Fac.ToString("yyyy-MM-dd"));
            xmlWriter.WriteElementString("ClienteId", factura.Id_Cli_Per.ToString());

            if (factura.Cliente != null)
            {
                xmlWriter.WriteStartElement("Cliente");
                xmlWriter.WriteElementString("Nombre", factura.Cliente.Nombre ?? "");
                xmlWriter.WriteElementString("Apellido", factura.Cliente.Apellido ?? "");
                xmlWriter.WriteElementString("Documento", factura.Cliente.Num_Documento ?? "");
                xmlWriter.WriteEndElement();
            }

            // Detalles
            xmlWriter.WriteStartElement("Detalles");
            if (factura.Detalles != null && factura.Detalles.Any())
            {
                foreach (var detalle in factura.Detalles)
                {
                    xmlWriter.WriteStartElement("Detalle");
                    xmlWriter.WriteElementString("ProductoId", detalle.Id_Pro_Per.ToString());
                    xmlWriter.WriteElementString("Cantidad", detalle.Cantidad_Comprada.ToString());
                    xmlWriter.WriteElementString("PrecioUnitario", ((decimal)detalle.Precio_Venta_Unit).ToString("F2"));
                    xmlWriter.WriteElementString("Descuento", ((decimal)detalle.Porcentaje_Descuento).ToString("F2"));
                    xmlWriter.WriteElementString("Total", ((decimal)detalle.Precio_Venta_Total).ToString("F2"));
                    xmlWriter.WriteEndElement();
                }
            }
            xmlWriter.WriteEndElement();

            // Totales
            xmlWriter.WriteStartElement("Totales");
            xmlWriter.WriteElementString("Subtotal", ((decimal)factura.Tot_Fac_Sin_IVA).ToString("F2"));
            xmlWriter.WriteElementString("IVA", ((decimal)factura.IVA_Fac).ToString("F2"));
            xmlWriter.WriteElementString("Total", ((decimal)factura.Tot_Fac_Con_IVA).ToString("F2"));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();

            return stringWriter.ToString();
        }

        private string GenerarHash(string contenido)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(contenido);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string FirmarConCertificado(string contenido, X509Certificate2 certificado)
        {
            if (certificado.PrivateKey == null)
                throw new BusinessValidationException("El certificado no tiene clave privada");

            byte[] data = Encoding.UTF8.GetBytes(contenido);
            
            using var rsa = certificado.GetRSAPrivateKey();
            if (rsa == null)
                throw new BusinessValidationException("No se pudo obtener la clave RSA del certificado");

            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }
    }
}
