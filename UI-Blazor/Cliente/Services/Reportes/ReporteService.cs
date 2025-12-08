using Application.DTOs.Reportes;
using System.Net.Http.Json;
using System.Web;

namespace Cliente.Services.Reportes
{
    public class ReporteService : IReporteService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/reportes";

        public ReporteService(HttpClient http) => _http = http;

        public async Task<ReporteFinancieroDto> GetReporteFinancieroAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var query = BuildQueryString(fechaInicio, fechaFin);
            try 
            {
                return await _http.GetFromJsonAsync<ReporteFinancieroDto>($"{BaseUrl}/financiero{query}") ?? new ReporteFinancieroDto();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error fetching Financial Report: {ex.Message}");
                return new ReporteFinancieroDto();
            }
        }

        public async Task<ReporteVentasInventarioDto> GetReporteVentasInventarioAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var query = BuildQueryString(fechaInicio, fechaFin);
             try 
            {
                return await _http.GetFromJsonAsync<ReporteVentasInventarioDto>($"{BaseUrl}/ventas-inventario{query}") ?? new ReporteVentasInventarioDto();
            }
            catch(Exception ex)
            {
                 Console.WriteLine($"Error fetching Sales Report: {ex.Message}");
                 return new ReporteVentasInventarioDto();
            }
        }

        private string BuildQueryString(DateTime? inicio, DateTime? fin)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (inicio.HasValue) query["fechaInicio"] = inicio.Value.ToString("yyyy-MM-dd");
            if (fin.HasValue) query["fechaFin"] = fin.Value.ToString("yyyy-MM-dd");
            return query.Count > 0 ? "?" + query.ToString() : "";
        }
    }
}
