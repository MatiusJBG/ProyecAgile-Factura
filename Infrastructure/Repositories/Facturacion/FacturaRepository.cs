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

        public async Task<(IEnumerable<Factura> Items, int TotalCount)> GetFacturasPagedAsync(int page, int pageSize, string searchTerm = "", string estado = "")
        {
            var query = _dbSet
                .Include(f => f.Cliente)
                .Include(f => f.Detalles)
                    .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                var isDate = DateTime.TryParse(searchTerm, out var searchDate);
                var isNumber = int.TryParse(searchTerm, out var searchNumber);
                
                query = query.Where(f => 
                    f.Id_Fac.ToString().Contains(searchTerm) || // Buscar por ID Factura
                    f.Cliente.Nombre.ToLower().Contains(searchLower) || 
                    (f.Cliente.Apellido != null && f.Cliente.Apellido.ToLower().Contains(searchLower)) ||
                    f.Cliente.Num_Documento.Contains(searchTerm) || // Buscar por Documento/Cédula
                    (isDate && f.Fec_Fac.Date == searchDate.Date) || // Buscar por Fecha Exacta
                    (isNumber && (f.Fec_Fac.Year == searchNumber || f.Fec_Fac.Month == searchNumber || f.Fec_Fac.Day == searchNumber)) // Buscar por Año, Mes o Día
                );
            }

            // Filtro de estado eliminado temporalmente por falta de propiedad en Entidad
            /*
            if (!string.IsNullOrWhiteSpace(estado) && Enum.TryParse<Core.Enums.Facturacion.EstadoFactura>(estado, out var estadoEnum))
            {
                query = query.Where(f => f.Estado == estadoEnum);
            }
            */

            // Ordenar por fecha descendente por defecto
            // Ordenar por fecha descendente por defecto, y luego por ID para estabilidad
            query = query.OrderByDescending(f => f.Fec_Fac).ThenByDescending(f => f.Id_Fac);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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
