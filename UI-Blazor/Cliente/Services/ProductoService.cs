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
                // Mock data for development
                return new List<Producto>
                {
                    new Producto { Id = 1, CodigoPrincipal = "PROD001", Nombre = "Producto A", TipoProducto = TipoProducto.BIEN, NumeroLote = "LOTE-001", CantidadEnLote = 100, PrecioLote = 10000.00m, IVA = TipoIVA.Gravado12, Stock = 50, Activo = true },
                    new Producto { Id = 2, CodigoPrincipal = "PROD002", Nombre = "Servicio B", TipoProducto = TipoProducto.SERVICIO, NumeroLote = "LOTE-002", CantidadEnLote = 1, PrecioLote = 200.00m, IVA = TipoIVA.Gravado12, Activo = true }
                };
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
            try
            {
                var response = await _http.PostAsJsonAsync(BaseUrl, producto);
                return await response.Content.ReadFromJsonAsync<Producto>() ?? producto;
            }
            catch
            {
                producto.Id = new Random().Next(1000, 9999);
                return producto;
            }
        }

        public async Task UpdateProductoAsync(Producto producto)
        {
            try
            {
                await _http.PutAsJsonAsync($"{BaseUrl}/{producto.Id}", producto);
            }
            catch
            {
                // Mock update
            }
        }

        public async Task DeleteProductoAsync(int id)
        {
            try
            {
                await _http.DeleteAsync($"{BaseUrl}/{id}");
            }
            catch
            {
                // Mock delete
            }
        }
    }
}

