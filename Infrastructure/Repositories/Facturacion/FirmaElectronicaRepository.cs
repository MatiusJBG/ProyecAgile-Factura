using Infrastructure.Repositories.Common;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Facturacion
{
    public class FirmaElectronicaRepository : Repository<FirmaElectronica>, IFirmaElectronicaRepository
    {
        public FirmaElectronicaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<FirmaElectronica?> GetByFacturaIdAsync(int idFactura)
        {
            return await _context.FirmasElectronicas
                .FirstOrDefaultAsync(f => f.Id_Fac_Per == idFactura);
        }
    }
}
