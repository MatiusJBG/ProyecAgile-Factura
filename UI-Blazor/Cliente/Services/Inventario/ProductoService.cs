using Application.DTOs.Producto;
using System.Net.Http.Json;

namespace Cliente.Services.Inventario
{
    public interface IProductoService
    {
        Task<List<ProductoDto>> GetProductosAsync();
        Task<ProductoDto?> GetProductoByIdAsync(int id);
        Task<ProductoDto?> GetProductoByNombreAsync(string nombre);
        Task<ProductoDto> CreateProductoAsync(ProductoDto producto);
        Task UpdateProductoAsync(ProductoDto producto);
        Task AddLoteAsync(int productoId, LoteDto lote);
        Task DeleteProductoAsync(int id);
    }

    public class ProductoService : IProductoService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/productos";

        public ProductoService(HttpClient http) => _http = http;

        public async Task<List<ProductoDto>> GetProductosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<ProductoDto>>(BaseUrl) ?? new();
            }
            catch
            {
                // Retorna lista vacía si backend no disponible (para desarrollo)
                return new List<ProductoDto>();
            }
        }

        public async Task<ProductoDto?> GetProductoByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<ProductoDto>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProductoDto?> GetProductoByNombreAsync(string nombre)
        {
            try
            {
                return await _http.GetFromJsonAsync<ProductoDto>($"{BaseUrl}/buscar/{nombre}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProductoDto> CreateProductoAsync(ProductoDto producto)
        {
            var response = await _http.PostAsJsonAsync(BaseUrl, producto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductoDto>() ?? producto;
        }

        public async Task UpdateProductoAsync(ProductoDto producto)
        {
            var response = await _http.PutAsJsonAsync($"{BaseUrl}/{producto.Id_Pro}", producto);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddLoteAsync(int productoId, LoteDto lote)
        {
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/{productoId}/lotes", lote);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductoAsync(int id)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
