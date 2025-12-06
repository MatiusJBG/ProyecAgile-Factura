using Core.Entities.Clientes;
using Core.Interfaces.Common;
using Core.Entities;

namespace Core.Interfaces.Clientes
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<Cliente?> GetByDocumentoAsync(string numDocumento);
    }
}

