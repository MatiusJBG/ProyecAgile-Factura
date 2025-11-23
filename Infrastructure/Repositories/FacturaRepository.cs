using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FacturaRepository : Repository<Factura>, IFacturaRepository
    {
        public FacturaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Factura?> GetFacturaWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(f => f.Cliente)
                .Include(f => f.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(f => f.Detalles)
                    .ThenInclude(d => d.Lote)
                .FirstOrDefaultAsync(f => f.Id_Fac == id);
        }

        public async Task<IEnumerable<Factura>> GetFacturasWithClienteAsync()
        {
            return await _dbSet
                .Include(f => f.Cliente)
                .Include(f => f.Detalles)
                .ToListAsync();
        }

        public override async Task<Factura?> GetByIdAsync(int id)
        {
            return await GetFacturaWithDetailsAsync(id);
        }

        public override async Task<IEnumerable<Factura>> GetAllAsync()
        {
            return await GetFacturasWithClienteAsync();
        }
    }
}
