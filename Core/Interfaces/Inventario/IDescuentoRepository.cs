using Core.Entities.Inventario;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Inventario
{
    public interface IDescuentoRepository : IRepository<DescuentoProducto>
    {
        Task<IEnumerable<DescuentoProducto>> GetAllActiveAsync();
        Task<IEnumerable<DescuentoProducto>> GetByProductoAsync(int idProducto);
        Task<DescuentoProducto?> GetActiveByProductoAsync(int idProducto);
    }
}

