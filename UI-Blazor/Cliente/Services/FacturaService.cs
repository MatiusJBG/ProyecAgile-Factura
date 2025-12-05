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
            // Mapear Models.Factura a FacturaDto (el modelo que espera el backend)
            var facturaDto = new
            {
                Id_Fac = factura.Id_Fac,
                Fec_Fac = factura.Fec_Fac,
                Id_Cli_Per = factura.Id_Cli_Per,
                Tot_Descuento = factura.Tot_Descuento,
                Tot_Fac_Sin_IVA = factura.Tot_Fac_Sin_IVA,
                IVA_Fac = factura.IVA_Fac,
                Tot_Fac_Con_IVA = factura.Tot_Fac_Con_IVA,
                Detalles = factura.Detalles.Select(d => new
                {
                    Id_Det_Fac = d.Id_Det_Fac,
                    Id_Fac_Per = d.Id_Fac_Per,
                    Id_Lote_Per = d.Id_Lote_Per,
                    Id_Pro_Per = d.Id_Pro_Per,
                    Cantidad_Comprada = d.Cantidad_Comprada,
                    Precio_Venta_Unit = d.Precio_Venta_Unit,
                    Porcentaje_Descuento = d.Porcentaje_Descuento,
                    Precio_Venta_Total = d.Precio_Venta_Total
                }).ToList()
            };
            
            var response = await _http.PostAsJsonAsync(BaseUrl, facturaDto);
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