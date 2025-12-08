
using Application.DTOs.Cliente;
using Application.DTOs.Producto;

namespace Application.Interfaces
{
    public interface IFileCacheService
    {
        Task SaveClientesCacheAsync(IEnumerable<ClienteDto> clientes);
        Task<IEnumerable<ClienteDto>> GetClientesCacheAsync();
        Task InvalidateClientesCacheAsync();
        
        Task SaveProductosCacheAsync(IEnumerable<ProductoDto> productos);
        Task<IEnumerable<ProductoDto>> GetProductosCacheAsync();
        Task InvalidateProductosCacheAsync();
        
        Task<bool> IsCacheValidAsync();
    }
}
