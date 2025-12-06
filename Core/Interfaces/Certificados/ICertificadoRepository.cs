using Core.Entities.Certificados;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Certificados
{
    /// <summary>
    /// Repositorio para operaciones de CertificadoDigital
    /// </summary>
    public interface ICertificadoRepository : IRepository<CertificadoDigital>
    {
        /// <summary>
        /// Obtiene el certificado digital activo y no expirado
        /// </summary>
        Task<CertificadoDigital?> GetActivoAsync();

        /// <summary>
        /// Obtiene todos los certificados ordenados por activo y fecha de carga
        /// </summary>
        Task<List<CertificadoDigital>> GetAllOrderedAsync();

        /// <summary>
        /// Obtiene todos los certificados activos
        /// </summary>
        Task<List<CertificadoDigital>> GetAllActivosAsync();
    }
}

