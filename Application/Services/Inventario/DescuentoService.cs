using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;

namespace Application.Services.Inventario
{
    public class DescuentoService
    {
        private readonly IDescuentoRepository _descuentoRepository;

        public DescuentoService(IDescuentoRepository descuentoRepository)
        {
            _descuentoRepository = descuentoRepository;
        }

        public async Task<IEnumerable<DescuentoProducto>> GetAllActiveDescuentosAsync()
        {
            return await _descuentoRepository.GetAllActiveAsync();
        }

        public async Task<IEnumerable<DescuentoProducto>> GetDescuentosByProductoAsync(int idProducto)
        {
            return await _descuentoRepository.GetByProductoAsync(idProducto);
        }

        public async Task<DescuentoProducto?> GetActiveDescuentoByProductoAsync(int idProducto)
        {
            return await _descuentoRepository.GetActiveByProductoAsync(idProducto);
        }

        public async Task<DescuentoProducto> CreateDescuentoAsync(DescuentoProducto descuento)
        {
            return await _descuentoRepository.AddAsync(descuento);
        }

        public async Task UpdateDescuentoAsync(DescuentoProducto descuento)
        {
            await _descuentoRepository.UpdateAsync(descuento);
        }

        public async Task DeleteDescuentoAsync(int id)
        {
            var descuento = await _descuentoRepository.GetByIdAsync(id);
            if (descuento != null)
            {
                await _descuentoRepository.DeleteAsync(descuento);
            }
        }
        
        public async Task<DescuentoProducto?> GetByIdAsync(int id)
        {
            return await _descuentoRepository.GetByIdAsync(id);
        }
    }
}
