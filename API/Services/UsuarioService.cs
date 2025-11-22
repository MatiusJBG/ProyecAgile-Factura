using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> LoginAsync(string username, string password)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Nom_Usu == username && u.Contrasena_Usu == password);
        }
        public async Task<IEnumerable<Usuario>> GetAllAsync()
{
    return await _context.Usuarios.ToListAsync();
}

    }
}
