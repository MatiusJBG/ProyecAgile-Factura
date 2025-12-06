using Core.Entities.Facturacion;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Facturacion
{
    /// <summary>
    /// Repositorio para operaciones de FirmaElectronica
    /// </summary>
    public interface IFirmaElectronicaRepository : IRepository<FirmaElectronica>
    {
        /// <summary>
        /// Obtiene la firma electrónica asociada a una factura
        /// </summary>
        Task<FirmaElectronica?> GetByFacturaIdAsync(int idFactura);
    }
}

