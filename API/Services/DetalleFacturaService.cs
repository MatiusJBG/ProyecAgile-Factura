using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace API.Services
{
    public class DetalleFacturaService
    {
        private readonly AppDbContext _context;

        public DetalleFacturaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DetalleFactura>> GetAllAsync()
        {
            return await _context.DetallesFactura.ToListAsync();
        }

        public async Task<DetalleFactura?> GetByIdAsync(int id)
        {
            return await _context.DetallesFactura.FindAsync(id);
        }
    }
}
