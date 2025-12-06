using Core.Interfaces.Common;
using Core.Entities.Auth;

namespace Core.Interfaces.Auth
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUsernameAsync(string username);
        Task<Usuario?> ValidateCredentialsAsync(string username, string password);
    }
}

