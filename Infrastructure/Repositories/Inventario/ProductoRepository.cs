using Infrastructure.Repositories.Common;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Inventario
{
    public class ProductoRepository : Repository<Producto>, IProductoRepository
    {
        public ProductoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Producto>> GetProductosWithStockAsync()
        {
            return await _dbSet
                .Include(p => p.Lotes)
                .Include(p => p.Precios)
                .ToListAsync();
        }

        public override async Task<Producto?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Lotes)
                .FirstOrDefaultAsync(p => p.Id_Pro == id);
        }

        public async Task<Producto?> GetByNombreAsync(string nombre)
        {
            return await _dbSet
                .Include(p => p.Lotes)
                .FirstOrDefaultAsync(p => p.Nom_Pro.ToLower() == nombre.ToLower());
        }

        public async Task<IEnumerable<Producto>> GetProductosForConsolidationAsync()
        {
            return await _dbSet
                .Include(p => p.Lotes)
                .Include(p => p.Precios)
                .ToListAsync();
        }
    }
}
