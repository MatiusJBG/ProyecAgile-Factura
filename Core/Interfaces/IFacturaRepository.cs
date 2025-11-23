using Core.Entities;

namespace Core.Interfaces
{
    public interface IFacturaRepository : IRepository<Factura>
    {
        Task<Factura?> GetFacturaWithDetailsAsync(int id);
        Task<IEnumerable<Factura>> GetFacturasWithClienteAsync();
    }
}
