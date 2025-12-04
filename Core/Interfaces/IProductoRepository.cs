using Core.Entities;

namespace Core.Interfaces
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<IEnumerable<Producto>> GetProductosWithStockAsync();
        Task<Producto?> GetByNombreAsync(string nombre);
    }
}
