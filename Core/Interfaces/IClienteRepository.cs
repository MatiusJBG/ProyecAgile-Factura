using Core.Entities;

namespace Core.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<Cliente?> GetByDocumentoAsync(string numDocumento);
    }
}
