using Application.DTOs.Producto;
using System.Net.Http.Json;

namespace Cliente.Services.Inventario
{
    public interface IDescuentoService
    {
        Task<List<DescuentoProductoDto>> GetAllDescuentosAsync();
        Task<List<DescuentoProductoDto>> GetDescuentosPorProductoAsync(int idProducto);
        Task<DescuentoProductoDto?> GetDescuentoActivoAsync(int idProducto);
        Task<DescuentoProductoDto> CreateDescuentoAsync(DescuentoProductoDto descuento);
        Task UpdateDescuentoAsync(DescuentoProductoDto descuento);
        Task DeleteDescuentoAsync(int id);
    }

    public class DescuentoService : IDescuentoService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/descuentos";

        public DescuentoService(HttpClient http) => _http = http;

        public async Task<List<DescuentoProductoDto>> GetAllDescuentosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<DescuentoProductoDto>>(BaseUrl) ?? new();
            }
            catch
            {
                return new List<DescuentoProductoDto>();
            }
        }

        public async Task<List<DescuentoProductoDto>> GetDescuentosPorProductoAsync(int idProducto)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<DescuentoProductoDto>>($"{BaseUrl}/producto/{idProducto}") ?? new();
            }
            catch
            {
                return new List<DescuentoProductoDto>();
            }
        }

        public async Task<DescuentoProductoDto?> GetDescuentoActivoAsync(int idProducto)
        {
            try
            {
                var response = await _http.GetAsync($"{BaseUrl}/activos/producto/{idProducto}");
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
                return await response.Content.ReadFromJsonAsync<DescuentoProductoDto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<DescuentoProductoDto> CreateDescuentoAsync(DescuentoProductoDto descuento)
        {
            var response = await _http.PostAsJsonAsync(BaseUrl, descuento);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DescuentoProductoDto>() ?? descuento;
        }

        public async Task UpdateDescuentoAsync(DescuentoProductoDto descuento)
        {
            var response = await _http.PutAsJsonAsync($"{BaseUrl}/{descuento.Id_Desc}", descuento);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteDescuentoAsync(int id)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
