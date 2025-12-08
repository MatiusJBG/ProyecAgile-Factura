using Application.DTOs.Factura;
using Application.DTOs.Common;
using System.Net.Http.Json;

namespace Cliente.Services.Facturacion
{
    public interface IFacturaService
    {
        Task<PagedResult<FacturaDto>> GetFacturasAsync(int page = 1, int pageSize = 10, string searchTerm = "", string estado = "");
        Task<FacturaDto?> GetFacturaAsync(int id);
        Task<FacturaStatsDto> GetStatsAsync();
        Task<FacturaDto> CreateAsync(FacturaDto factura);
        Task UpdateAsync(FacturaDto factura);
        Task DeleteAsync(int id);
    }

    public class FacturaService : IFacturaService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/facturas";

        public FacturaService(HttpClient http) => _http = http;

        public async Task<FacturaStatsDto> GetStatsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<FacturaStatsDto>($"{BaseUrl}/stats") ?? new FacturaStatsDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FACTURA SERVICE ERROR] GetStatsAsync failed: {ex.Message}");
                return new FacturaStatsDto();
            }
        }

        public async Task<PagedResult<FacturaDto>> GetFacturasAsync(int page = 1, int pageSize = 10, string searchTerm = "", string estado = "")
        {
            try
            {
                var url = $"{BaseUrl}?page={page}&pageSize={pageSize}&searchTerm={searchTerm}&estado={estado}";
                return await _http.GetFromJsonAsync<PagedResult<FacturaDto>>(url) ?? new PagedResult<FacturaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FACTURA SERVICE ERROR] GetFacturasAsync failed: {ex.Message}");
                return new PagedResult<FacturaDto>();
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