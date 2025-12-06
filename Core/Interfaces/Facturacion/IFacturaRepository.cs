using Core.Entities.Facturacion;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Facturacion
{
    public interface IFacturaRepository : IRepository<Factura>
    {
        Task<Factura?> GetFacturaWithDetailsAsync(int id);
        Task<IEnumerable<Factura>> GetFacturasWithClienteAsync();
    }
}

