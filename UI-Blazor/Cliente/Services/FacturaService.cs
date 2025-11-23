using UI_Blazor.Client.Models;
using System.Net.Http.Json;
using Models = UI_Blazor.Client.Models;

namespace UI_Blazor.Client.Services
{
    public interface IFacturaService
    {
        Task<List<Models.Factura>> GetFacturasAsync();
        Task<Models.Factura?> GetFacturaAsync(int id);
        Task<Models.Factura> CreateAsync(Models.Factura factura);
        Task UpdateAsync(Models.Factura factura);
        Task DeleteAsync(int id);
    }

    public class FacturaService : IFacturaService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/facturas";

        public FacturaService(HttpClient http) => _http = http;

        public async Task<List<Models.Factura>> GetFacturasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Models.Factura>>(BaseUrl) ?? new();
            }
            catch
            {
                // Retorna lista vacía si backend no disponible (para desarrollo)
                return new List<Models.Factura>();
            }
        }

        public async Task<Models.Factura?> GetFacturaAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Models.Factura>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.Factura> CreateAsync(Models.Factura factura)
        {
            var response = await _http.PostAsJsonAsync(BaseUrl, factura);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Models.Factura>() ?? factura;
        }

        public async Task UpdateAsync(Models.Factura factura)
        {
            var response = await _http.PutAsJsonAsync($"{BaseUrl}/{factura.Id_Fac}", factura);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}