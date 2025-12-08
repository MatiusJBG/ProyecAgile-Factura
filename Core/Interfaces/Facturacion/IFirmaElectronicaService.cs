using Core.Interfaces.Common;
using Core.Entities.Facturacion;

namespace Core.Interfaces.Facturacion
{
    /// <summary>
    /// Servicio para operaciones de firma electrónica de facturas
    /// </summary>
    public interface IFirmaElectronicaService
    {
        /// <summary>
        /// Firma una factura digitalmente
        /// </summary>
        Task<FirmaElectronica> FirmarFacturaAsync(int idFactura);

        /// <summary>
        /// Valida la firma de una factura
        /// </summary>
        Task<bool> ValidarFirmaAsync(int idFactura);

        /// <summary>
        /// Obtiene la firma electrónica de una factura
        /// </summary>
        Task<FirmaElectronica?> GetFirmaPorFacturaAsync(int idFactura);

        /// <summary>
        /// Genera el XML de una factura para firma
        /// </summary>
        string GenerarXmlFactura(Factura factura);

        /// <summary>
        /// Firma un XML (bytes) usando XAdES-BES (Enveloped) para el SRI
        /// </summary>
        byte[] FirmarXmlSri(byte[] xmlBytes, System.Security.Cryptography.X509Certificates.X509Certificate2 certificado);
    }
}

