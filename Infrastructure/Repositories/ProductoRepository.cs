using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
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
                .ToListAsync();
        }

        public override async Task<Producto?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Lotes)
                .FirstOrDefaultAsync(p => p.Id_Pro == id);
        }
    }
}
