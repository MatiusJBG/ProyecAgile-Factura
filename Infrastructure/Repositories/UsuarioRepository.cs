using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Nom_Usu == username);
        }

        public async Task<Usuario?> ValidateCredentialsAsync(string username, string password)
        {
            // En un entorno real, la contraseña debería estar hasheada.
            // Aquí asumimos comparación directa por simplicidad o hash simple si ya está implementado.
            // TODO: Implementar hashing de contraseñas seguro (BCrypt/Argon2)
            
            var usuario = await GetByUsernameAsync(username);
            
            if (usuario != null && usuario.Contrasena_Usu == password)
            {
                return usuario;
            }

            return null;
        }
    }
}
