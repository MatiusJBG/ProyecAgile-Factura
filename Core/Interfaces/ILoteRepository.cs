using Core.Entities;

namespace Core.Interfaces
{
    public interface ILoteRepository : IRepository<Lote>
    {
        Task<IEnumerable<Lote>> GetLotesByProductoAsync(int idProducto);
        Task<IEnumerable<Lote>> GetLotesDisponiblesAsync();
    }
}
