using Core.Entities;

namespace Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUsernameAsync(string username);
        Task<Usuario?> ValidateCredentialsAsync(string username, string password);
    }
}
