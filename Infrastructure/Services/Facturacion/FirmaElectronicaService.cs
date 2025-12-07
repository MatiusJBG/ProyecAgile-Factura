using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Exceptions;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Security.Cryptography.Xml;

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
        public byte[] FirmarXmlSri(byte[] xmlBytes, X509Certificate2 certificado)
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true; // CRITICAL for signature validity
            using (var ms = new MemoryStream(xmlBytes))
            {
                doc.Load(ms);
            }

            // 1. Create SignedXml with proper ID resolution
            var signedXml = new SriSignedXml(doc) { SigningKey = certificado.GetRSAPrivateKey() };
            // SHA256 - Required for 2025 standards
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
            // Use Exclusive Canonicalization (Robust against namespace issues)
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.Signature.Id = "Signature-SRI";

            // 2. Main Reference (The Invoice) - Enveloped + ExcC14N + SHA256
            var reference = new Reference { Uri = "#comprobante" };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            // Use Exclusive C14N for the reference transform as well
            reference.AddTransform(new XmlDsigExcC14NTransform()); 
            reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256"; // SHA256
            signedXml.AddReference(reference);

            // 3. KeyInfo - X509Data + RSAKeyValue (Broadest Compatibility)
            var keyInfo = new KeyInfo();
            var kix = new KeyInfoX509Data(certificado);
            kix.AddSubjectName(certificado.Subject);
            keyInfo.AddClause(kix);
            keyInfo.AddClause(new RSAKeyValue(certificado.GetRSAPrivateKey()));
            signedXml.KeyInfo = keyInfo;

            // 4. XAdES-BES Construction (DOM-based for namespace hygiene)
            var xadesDoc = new XmlDocument();
            var xadesNs = "http://uri.etsi.org/01903/v1.3.2#";
            var dsNs = "http://www.w3.org/2000/09/xmldsig#";

            var qualProps = xadesDoc.CreateElement("xades", "QualifyingProperties", xadesNs);
            qualProps.SetAttribute("Target", "#Signature-SRI");

            var signedProps = xadesDoc.CreateElement("xades", "SignedProperties", xadesNs);
            var signedPropsId = "SignedProperties-" + Guid.NewGuid().ToString();
            signedProps.SetAttribute("Id", signedPropsId);

            var signedSigProps = xadesDoc.CreateElement("xades", "SignedSignatureProperties", xadesNs);

            // 4a. SigningTime
            var signingTime = xadesDoc.CreateElement("xades", "SigningTime", xadesNs);
            signingTime.InnerText = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
            signedSigProps.AppendChild(signingTime);

            // 4b. SigningCertificate
            var signingCert = xadesDoc.CreateElement("xades", "SigningCertificate", xadesNs);
            var certElem = xadesDoc.CreateElement("xades", "Cert", xadesNs);
            
            // CertDigest - SHA256
            var certDigest = xadesDoc.CreateElement("xades", "CertDigest", xadesNs);
            var digestMethod = xadesDoc.CreateElement("ds", "DigestMethod", dsNs);
            digestMethod.SetAttribute("Algorithm", "http://www.w3.org/2001/04/xmlenc#sha256"); // SHA256
            var digestValue = xadesDoc.CreateElement("ds", "DigestValue", dsNs);
            using (var sha256 = SHA256.Create())
            {
                digestValue.InnerText = Convert.ToBase64String(sha256.ComputeHash(certificado.RawData));
            }
            certDigest.AppendChild(digestMethod);
            certDigest.AppendChild(digestValue);
            certElem.AppendChild(certDigest);

            // IssuerSerial
            var issuerSerial = xadesDoc.CreateElement("xades", "IssuerSerial", xadesNs);
            var issuerName = xadesDoc.CreateElement("ds", "X509IssuerName", dsNs);
            issuerName.InnerText = certificado.IssuerName.Name;
            var serialNumber = xadesDoc.CreateElement("ds", "X509SerialNumber", dsNs);
            
            // Safe Serial Parsing (Force Positive)
            var hexSerial = certificado.SerialNumber;
            if (long.TryParse(hexSerial.Substring(0, 1), System.Globalization.NumberStyles.HexNumber, null, out long firstDigit))
            {
                if (firstDigit >= 8) hexSerial = "0" + hexSerial;
            }
            if (!hexSerial.StartsWith("0")) hexSerial = "0" + hexSerial;

            var serialBigInt = System.Numerics.BigInteger.Parse(hexSerial, System.Globalization.NumberStyles.AllowHexSpecifier);
            serialNumber.InnerText = serialBigInt.ToString();
            
            issuerSerial.AppendChild(issuerName);
            issuerSerial.AppendChild(serialNumber);
            certElem.AppendChild(issuerSerial);

            signingCert.AppendChild(certElem);
            signedSigProps.AppendChild(signingCert);
            
            signedProps.AppendChild(signedSigProps);
            qualProps.AppendChild(signedProps);
            xadesDoc.AppendChild(qualProps);

            // 5. Add XAdES Object to SignedXml
            var objectNode = new DataObject();
            var importedQualProps = doc.ImportNode(xadesDoc.DocumentElement, true);
            var dummy = doc.CreateElement("dummy");
            dummy.AppendChild(importedQualProps);
            objectNode.Data = dummy.ChildNodes;
            signedXml.AddObject(objectNode);

            // Register for ID resolution
            signedXml.AddExternalElement((XmlElement)importedQualProps);

            // 6. XAdES Reference - SHA256 + Exclusive C14N
            var referenceXades = new Reference { Uri = "#" + signedPropsId, Type = "http://uri.etsi.org/01903#SignedProperties" };
            // Use Exclusive C14N
            referenceXades.AddTransform(new XmlDsigExcC14NTransform());
            referenceXades.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256"; // SHA256
            signedXml.AddReference(referenceXades);

            // 7. Compute & Save
            signedXml.ComputeSignature();
            var xmlDigitalSignature = signedXml.GetXml();
            
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            using (var msOutput = new MemoryStream())
            {
                var settings = new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = false };
                using (var writer = XmlWriter.Create(msOutput, settings))
                {
                    doc.Save(writer);
                }
                return msOutput.ToArray();
            }
        }
    }

    // Custom SignedXml to handle detached/object IDs
    internal class SriSignedXml : SignedXml
    {
        private List<XmlElement> _externalElements = new List<XmlElement>();

        public SriSignedXml(XmlDocument document) : base(document) { }

        public void AddExternalElement(XmlElement element)
        {
            _externalElements.Add(element);
        }

        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            // First check standard document
            var elem = base.GetIdElement(document, idValue);
            if (elem != null) return elem;

            // Then check external elements (our XAdES object)
            foreach (var extElem in _externalElements)
            {
                if (extElem.GetAttribute("Id") == idValue) return extElem;
                
                // Use NameTable to avoid prefix issues if possible, but localized check is safer
                var nsMgr = new XmlNamespaceManager(document.NameTable);
                nsMgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
                nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

                var found = extElem.SelectSingleNode($"//*[@Id='{idValue}']", nsMgr) as XmlElement;
                if (found != null) return found;
                
                // Fallback for no namespace match or arbitrary structure
                 var allElems = extElem.GetElementsByTagName("*");
                 foreach(XmlElement child in allElems)
                 {
                     if(child.GetAttribute("Id") == idValue) return child;
                 }
            }
            return null;
        }
    }
}
