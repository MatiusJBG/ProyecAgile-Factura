using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class FacturaService
    {
        private readonly AppDbContext _context;

        public FacturaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Factura>> GetAllAsync()
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetallesFactura)
                .ToListAsync();
        }

        public async Task<Factura?> GetByIdAsync(int id)
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetallesFactura)
                .FirstOrDefaultAsync(f => f.Id_Fac == id);
        }
        public async Task<Factura> CreateAsync(Factura factura)
{
    _context.Facturas.Add(factura);
    await _context.SaveChangesAsync();
    return factura;
}

    }
}
