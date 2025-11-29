using UI_Blazor.Client.Models;
using System.Net.Http.Json;

namespace UI_Blazor.Client.Services
{
    public interface IDescuentoService
    {
        Task<List<DescuentoProducto>> GetAllDescuentosAsync();
        Task<List<DescuentoProducto>> GetDescuentosPorProductoAsync(int idProducto);
        Task<DescuentoProducto?> GetDescuentoActivoAsync(int idProducto);
        Task<DescuentoProducto> CreateDescuentoAsync(DescuentoProducto descuento);
        Task UpdateDescuentoAsync(DescuentoProducto descuento);
        Task DeleteDescuentoAsync(int id);
    }

    public class DescuentoService : IDescuentoService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/descuentos";

        public DescuentoService(HttpClient http) => _http = http;

        public async Task<List<DescuentoProducto>> GetAllDescuentosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<DescuentoProducto>>(BaseUrl) ?? new();
            }
            catch
            {
                return new List<DescuentoProducto>();
            }
        }

        public async Task<List<DescuentoProducto>> GetDescuentosPorProductoAsync(int idProducto)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<DescuentoProducto>>($"{BaseUrl}/producto/{idProducto}") ?? new();
            }
            catch
            {
                return new List<DescuentoProducto>();
            }
        }

        public async Task<DescuentoProducto?> GetDescuentoActivoAsync(int idProducto)
        {
            try
            {
                var response = await _http.GetAsync($"{BaseUrl}/activos/producto/{idProducto}");
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
                return await response.Content.ReadFromJsonAsync<DescuentoProducto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<DescuentoProducto> CreateDescuentoAsync(DescuentoProducto descuento)
        {
            var response = await _http.PostAsJsonAsync(BaseUrl, descuento);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DescuentoProducto>() ?? descuento;
        }

        public async Task UpdateDescuentoAsync(DescuentoProducto descuento)
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
