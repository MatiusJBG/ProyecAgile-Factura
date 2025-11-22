using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace API.Services
{
    public class AuditoriaService
    {
        private readonly AppDbContext _context;

        public AuditoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Auditoria>> GetAllAsync()
        {
            return await _context.Auditorias.ToListAsync();
        }
    }
}
