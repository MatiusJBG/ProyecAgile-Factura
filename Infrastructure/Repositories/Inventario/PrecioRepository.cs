using Infrastructure.Repositories.Common;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Inventario
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
