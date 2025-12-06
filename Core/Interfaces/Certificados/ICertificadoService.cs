using Core.Interfaces.Common;
using Core.Entities.Certificados;
using System.Security.Cryptography.X509Certificates;

namespace Core.Interfaces.Certificados
{
    /// <summary>
    /// Servicio para operaciones de certificados digitales
    /// </summary>
    public interface ICertificadoService
    {
        /// <summary>
        /// Obtiene el certificado digital activo
        /// </summary>
        Task<CertificadoDigital?> GetCertificadoActivoAsync();

        /// <summary>
        /// Obtiene todos los certificados
        /// </summary>
        Task<List<CertificadoDigital>> GetAllCertificadosAsync();

        /// <summary>
        /// Sube un nuevo certificado digital
        /// </summary>
        Task<CertificadoDigital> SubirCertificadoAsync(byte[] archivoBytes, string password, string nombre);

        /// <summary>
        /// Activa un certificado digital
        /// </summary>
        Task<bool> ActivarCertificadoAsync(int idCertificado);

        /// <summary>
        /// Valida un certificado digital
        /// </summary>
        Task<bool> ValidarCertificadoAsync(int idCertificado);

        /// <summary>
        /// Carga un certificado X509 desde el archivo
        /// </summary>
        Task<X509Certificate2> CargarCertificadoX509Async(int idCertificado);

        /// <summary>
        /// Elimina un certificado digital
        /// </summary>
        Task EliminarCertificadoAsync(int idCertificado);
    }
}

