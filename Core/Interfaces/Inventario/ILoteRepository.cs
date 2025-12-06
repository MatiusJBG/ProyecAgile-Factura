using Core.Entities.Inventario;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Inventario
{
    public interface ILoteRepository : IRepository<Lote>
    {
        Task<IEnumerable<Lote>> GetLotesByProductoAsync(int idProducto);
        Task<IEnumerable<Lote>> GetLotesDisponiblesAsync();
    }
}

