
using Application.DTOs.Cliente;
using Application.DTOs.Producto;

namespace Application.Interfaces
{
    public interface IFileCacheService
    {
        Task SaveClientesCacheAsync(IEnumerable<ClienteDto> clientes);
        Task<IEnumerable<ClienteDto>> GetClientesCacheAsync();
        
        Task SaveProductosCacheAsync(IEnumerable<ProductoDto> productos);
        Task<IEnumerable<ProductoDto>> GetProductosCacheAsync();
        
        Task<bool> IsCacheValidAsync();
    }
}
