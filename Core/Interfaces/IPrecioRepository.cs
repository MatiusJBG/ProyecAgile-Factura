using Core.Entities;

namespace Core.Interfaces
{
    public interface IPrecioRepository : IRepository<Precio>
    {
        Task<Precio?> GetCurrentPriceAsync(int productoId);
        Task<IEnumerable<Precio>> GetPriceHistoryAsync(int productoId);
    }
}
