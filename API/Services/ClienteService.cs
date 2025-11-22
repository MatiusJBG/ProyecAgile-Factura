using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class ClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            return await _context.Clientes
                .Include(c => c.Facturas)
                .ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes
                .Include(c => c.Facturas)
                .FirstOrDefaultAsync(c => c.Id_Cli == id);
        }

        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            // Validar documento único
            if (await _context.Clientes.AnyAsync(x => x.Num_Documento == cliente.Num_Documento))
                throw new Exception("El número de documento ya está registrado.");

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            var existing = await _context.Clientes.FindAsync(cliente.Id_Cli);
            if (existing == null) return false;

            // SetValues = actualiza solo campos enviados
            _context.Entry(existing).CurrentValues.SetValues(cliente);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            _context.Clientes.Remove(cliente);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
