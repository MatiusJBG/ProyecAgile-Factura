using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PrecioRepository : Repository<Precio>, IPrecioRepository
    {
        public PrecioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Precio?> GetCurrentPriceAsync(int productoId)
        {
            return await _dbSet
                .Where(p => p.Id_Pro_Per == productoId)
                .OrderByDescending(p => p.Fecha_Actualizacion)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Precio>> GetPriceHistoryAsync(int productoId)
        {
            return await _dbSet
                .Where(p => p.Id_Pro_Per == productoId)
                .OrderByDescending(p => p.Fecha_Actualizacion)
                .ToListAsync();
        }
    }
}
