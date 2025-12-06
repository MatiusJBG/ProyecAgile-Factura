using Infrastructure.Repositories.Common;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Facturacion
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
                    .ThenInclude(d => d.Producto)
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
