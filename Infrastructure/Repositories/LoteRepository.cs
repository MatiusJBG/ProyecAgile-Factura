using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LoteRepository : Repository<Lote>, ILoteRepository
    {
        public LoteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Lote>> GetLotesByProductoAsync(int idProducto)
        {
            return await _dbSet
                .Where(l => l.Id_Pro_Per == idProducto)
                .Include(l => l.Producto)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lote>> GetLotesDisponiblesAsync()
        {
            return await _dbSet
                .Where(l => l.Cantidad_Disponible > 0)
                .Include(l => l.Producto)
                .ToListAsync();
        }

        public override async Task<Lote?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Producto)
                .FirstOrDefaultAsync(l => l.Id_Lote == id);
        }

        public override async Task<IEnumerable<Lote>> GetAllAsync()
        {
            return await _dbSet
                .Include(l => l.Producto)
                .ToListAsync();
        }
    }
}
