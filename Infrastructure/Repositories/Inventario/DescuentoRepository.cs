using Infrastructure.Repositories.Common;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Inventario
{
    public class DescuentoRepository : Repository<DescuentoProducto>, IDescuentoRepository
    {
        public DescuentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DescuentoProducto>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(d => d.Activo)
                .OrderByDescending(d => d.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<DescuentoProducto>> GetByProductoAsync(int idProducto)
        {
            return await _dbSet
                .Where(d => d.Id_Pro_Per == idProducto)
                .OrderByDescending(d => d.FechaInicio)
                .ToListAsync();
        }

        public async Task<DescuentoProducto?> GetActiveByProductoAsync(int idProducto)
        {
            var today = DateTime.Today;
            return await _dbSet
                .Where(d => d.Id_Pro_Per == idProducto && 
                            d.Activo && 
                            d.FechaInicio <= today && 
                            (d.FechaFin == null || d.FechaFin >= today))
                .OrderByDescending(d => d.FechaInicio)
                .FirstOrDefaultAsync();
        }
    }
}
