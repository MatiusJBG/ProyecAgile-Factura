using UI_Blazor.Client.Models;
using System.Net.Http.Json;

namespace UI_Blazor.Client.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> GetProductosAsync();
        Task<Producto?> GetProductoByIdAsync(int id);
        Task<Producto> CreateProductoAsync(Producto producto);
        Task UpdateProductoAsync(Producto producto);
        Task DeleteProductoAsync(int id);
    }

    public class ProductoService : IProductoService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/productos";

        public ProductoService(HttpClient http) => _http = http;

        public async Task<List<Producto>> GetProductosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Producto>>(BaseUrl) ?? new();
            }
            catch
            {
                // Retorna lista vacía si backend no disponible (para desarrollo)
                return new List<Producto>();
            }
        }

        public async Task<Producto?> GetProductoByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Producto>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<Producto> CreateProductoAsync(Producto producto)
        {
            var response = await _http.PostAsJsonAsync(BaseUrl, producto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Producto>() ?? producto;
        }

        public async Task UpdateProductoAsync(Producto producto)
        {
            var response = await _http.PutAsJsonAsync($"{BaseUrl}/{producto.Id_Pro}", producto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductoAsync(int id)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
