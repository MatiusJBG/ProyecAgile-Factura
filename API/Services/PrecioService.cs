using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace API.Services
{
    public class PrecioService
    {
        private readonly AppDbContext _context;

        public PrecioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Precio>> GetAllAsync()
        {
            return await _context.Precios.ToListAsync();
        }

        public async Task<Precio?> GetByIdAsync(int id)
        {
            return await _context.Precios.FindAsync(id);
        }

        // 🔥 FALTABA ESTE MÉTODO
        public async Task<Precio> CreateAsync(Precio precio)
        {
            _context.Precios.Add(precio);
            await _context.SaveChangesAsync();
            return precio;
        }
    }
}
