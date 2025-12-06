using Core.Entities.Inventario;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Inventario
{
    public interface IPrecioRepository : IRepository<Precio>
    {
        Task<Precio?> GetCurrentPriceAsync(int productoId);
        Task<IEnumerable<Precio>> GetPriceHistoryAsync(int productoId);
    }
}

