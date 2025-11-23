using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cliente?> GetByDocumentoAsync(string numDocumento)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Num_Documento == numDocumento);
        }
    }
}
