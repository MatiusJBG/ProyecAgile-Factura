using Core.Entities.Facturacion;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Facturacion
{
    public interface IFacturaRepository : IRepository<Factura>
    {
        Task<Factura?> GetFacturaWithDetailsAsync(int id);
        Task<IEnumerable<Factura>> GetFacturasWithClienteAsync();
        Task<(IEnumerable<Factura> Items, int TotalCount)> GetFacturasPagedAsync(int page, int pageSize, string searchTerm = "", string estado = "");
        
        Task<Factura?> GetByClaveAccesoAsync(string claveAcceso);
        Task<(int TotalCount, decimal TotalPagado)> GetFacturaStatsAsync();
    }
}

