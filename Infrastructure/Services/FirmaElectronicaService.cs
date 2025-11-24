using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public interface IFirmaElectronicaService
    {
        Task<FirmaElectronica> FirmarFacturaAsync(int idFactura);
        Task<bool> ValidarFirmaAsync(int idFactura);
        Task<FirmaElectronica?> GetFirmaPorFacturaAsync(int idFactura);
        string GenerarXmlFactura(Factura factura);
    }

    public class FirmaElectronicaService : IFirmaElectronicaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICertificadoService _certificadoService;
        private readonly IConfiguration _configuration;

        public FirmaElectronicaService(
            ApplicationDbContext context,
            ICertificadoService certificadoService,
            IConfiguration configuration)
        {
            _context = context;
            _certificadoService = certificadoService;
            _configuration = configuration;
        }

        public async Task<FirmaElectronica> FirmarFacturaAsync(int idFactura)
        {
            // Obtener la factura con sus detalles
            var factura = await _context.Facturas
                .Include(f => f.Detalles)
                .Include(f => f.Cliente)
                .FirstOrDefaultAsync(f => f.Id_Fac == idFactura);

            if (factura == null)
                throw new Exception($"Factura con ID {idFactura} no encontrada");

            // Verificar si ya está firmada
            var firmaExistente = await _context.FirmasElectronicas
                .FirstOrDefaultAsync(f => f.Id_Fac_Per == idFactura);

            if (firmaExistente != null)
                throw new Exception("La factura ya está firmada");

            // Obtener certificado activo
            var certificado = await _certificadoService.GetCertificadoActivoAsync();
            if (certificado == null)
                throw new Exception("No hay certificado activo configurado");

            // Generar XML de la factura
            string xmlFactura = GenerarXmlFactura(factura);

            // Cargar el certificado X509
            var cert = await _certificadoService.CargarCertificadoX509Async(certificado.Id_Cert);

            // Generar hash del XML
            string hashDocumento = GenerarHash(xmlFactura);

            // Firmar el hash
            string firmaDigital = FirmarConCertificado(xmlFactura, cert);

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

            _context.FirmasElectronicas.Add(firma);
            await _context.SaveChangesAsync();

            return firma;
        }

        public async Task<bool> ValidarFirmaAsync(int idFactura)
        {
            var firma = await _context.FirmasElectronicas
                .FirstOrDefaultAsync(f => f.Id_Fac_Per == idFactura);

            if (firma == null)
                return false;

            var factura = await _context.Facturas
                .Include(f => f.Detalles)
                .Include(f => f.Cliente)
                .FirstOrDefaultAsync(f => f.Id_Fac == idFactura);

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

            await _context.SaveChangesAsync();

            return hashCoincide;
        }

        public async Task<FirmaElectronica?> GetFirmaPorFacturaAsync(int idFactura)
        {
            return await _context.FirmasElectronicas
                .FirstOrDefaultAsync(f => f.Id_Fac_Per == idFactura);
        }

        // Infrastructure/Services/FirmaElectronicaService.cs

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
    foreach (var detalle in factura.Detalles)
    {
        xmlWriter.WriteStartElement("Detalle");
        xmlWriter.WriteElementString("ProductoId", detalle.Id_Pro_Per.ToString());
        xmlWriter.WriteElementString("Cantidad", detalle.Cantidad_Comprada.ToString());
        
        // CORRECCIÓN CS1501: Se utiliza (decimal) para forzar la sobrecarga de ToString
        xmlWriter.WriteElementString("PrecioUnitario", ((decimal)detalle.Precio_Venta_Unit).ToString("F2"));
        xmlWriter.WriteElementString("Descuento", ((decimal)detalle.Porcentaje_Descuento).ToString("F2"));
        xmlWriter.WriteElementString("Total", ((decimal)detalle.Precio_Venta_Total).ToString("F2"));
        
        xmlWriter.WriteEndElement();
    }
    xmlWriter.WriteEndElement();

    // Totales
    xmlWriter.WriteStartElement("Totales");
    // CORRECCIÓN CS1501
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
                throw new Exception("El certificado no tiene clave privada");

            byte[] data = Encoding.UTF8.GetBytes(contenido);
            
            using var rsa = certificado.GetRSAPrivateKey();
            if (rsa == null)
                throw new Exception("No se pudo obtener la clave RSA del certificado");

            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }
    }
}
