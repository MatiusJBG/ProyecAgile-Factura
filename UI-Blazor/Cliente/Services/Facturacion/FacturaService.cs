using Application.DTOs.Factura;
using System.Net.Http.Json;

namespace Cliente.Services.Facturacion
{
    public interface IFacturaService
    {
        Task<List<FacturaDto>> GetFacturasAsync();
        Task<FacturaDto?> GetFacturaAsync(int id);
        Task<FacturaDto> CreateAsync(FacturaDto factura);
        Task UpdateAsync(FacturaDto factura);
        Task DeleteAsync(int id);
    }

    public class FacturaService : IFacturaService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/facturas";

        public FacturaService(HttpClient http) => _http = http;

        public async Task<List<FacturaDto>> GetFacturasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<FacturaDto>>(BaseUrl) ?? new();
            }
            catch
            {
                // Retorna lista vacía si backend no disponible (para desarrollo)
                return new List<FacturaDto>();
            }
        }

        public async Task<FacturaDto?> GetFacturaAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<FacturaDto>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<FacturaDto> CreateAsync(FacturaDto factura)
        {
            // Mapeo directo del DTO
            var response = await _http.PostAsJsonAsync(BaseUrl, factura);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FacturaDto>() ?? factura;
        }

        public async Task UpdateAsync(FacturaDto factura)
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