using Core.Entities.Inventario;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Inventario
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<IEnumerable<Producto>> GetProductosWithStockAsync();
        Task<Producto?> GetByNombreAsync(string nombre);
        Task<IEnumerable<Producto>> GetProductosForConsolidationAsync();
    }
}

