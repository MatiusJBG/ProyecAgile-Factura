using Infrastructure.Repositories.Common;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Certificados
{
    public class CertificadoRepository : Repository<CertificadoDigital>, ICertificadoRepository
    {
        public CertificadoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CertificadoDigital?> GetActivoAsync()
        {
            return await _context.CertificadosDigitales
                .Where(c => c.Activo && c.Fecha_Expiracion > DateTime.UtcNow)
                .OrderByDescending(c => c.Fecha_Carga)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CertificadoDigital>> GetAllOrderedAsync()
        {
            return await _context.CertificadosDigitales
                .OrderByDescending(c => c.Activo)
                .ThenByDescending(c => c.Fecha_Carga)
                .ToListAsync();
        }

        public async Task<List<CertificadoDigital>> GetAllActivosAsync()
        {
            return await _context.CertificadosDigitales
                .Where(c => c.Activo)
                .ToListAsync();
        }
    }
}
