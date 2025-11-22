using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class LoteService
    {
        private readonly AppDbContext _context;

        public LoteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Lote>> GetAllAsync()
        {
            return await _context.Lotes
                .Include(l => l.Producto)
                .ToListAsync();
        }

        public async Task<Lote?> GetByIdAsync(int id)
        {
            return await _context.Lotes
                .Include(l => l.Producto)
                .FirstOrDefaultAsync(l => l.Id_Lote == id);
        }
        public async Task<Lote> CreateAsync(Lote lote)
{
    _context.Lotes.Add(lote);
    await _context.SaveChangesAsync();
    return lote;
}

public async Task<bool> UpdateAsync(Lote lote)
{
    var existe = await _context.Lotes.AnyAsync(x => x.Id_Lote == lote.Id_Lote);
    if (!existe) return false;

    _context.Lotes.Update(lote);
    await _context.SaveChangesAsync();
    return true;
}

public async Task<bool> DeleteAsync(int id)
{
    var l = await _context.Lotes.FindAsync(id);
    if (l == null) return false;

    _context.Lotes.Remove(l);
    await _context.SaveChangesAsync();
    return true;
}

    }
}
