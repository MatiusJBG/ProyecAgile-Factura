using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class ProductoService
    {
        private readonly AppDbContext _context;

        public ProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> GetAllAsync()
        {
            return await _context.Productos
                .Include(p => p.Precios)
                .Include(p => p.Lotes)
                .ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Precios)
                .Include(p => p.Lotes)
                .FirstOrDefaultAsync(p => p.Id_Pro == id);
        }
public async Task<Producto> CreateAsync(Producto producto)
{
    _context.Productos.Add(producto);
    await _context.SaveChangesAsync();
    return producto;
}

public async Task<bool> UpdateAsync(Producto producto)
{
    var existe = await _context.Productos.AnyAsync(x => x.Id_Pro == producto.Id_Pro);
    if (!existe) return false;

    _context.Productos.Update(producto);
    await _context.SaveChangesAsync();
    return true;
}

public async Task<bool> DeleteAsync(int id)
{
    var prod = await _context.Productos.FindAsync(id);
    if (prod == null) return false;

    _context.Productos.Remove(prod);
    await _context.SaveChangesAsync();
    return true;
}
}
}