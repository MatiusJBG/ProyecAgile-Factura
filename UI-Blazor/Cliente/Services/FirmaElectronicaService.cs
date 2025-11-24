using UI_Blazor.Client.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace UI_Blazor.Client.Services
{
    public interface IFirmaElectronicaService
    {
        Task<FirmaElectronica> FirmarFacturaAsync(int idFactura);
        Task<bool> ValidarFirmaAsync(int idFactura);
        Task<FirmaElectronica?> GetFirmaPorFacturaAsync(int idFactura);
    }

    public class FirmaElectronicaService : IFirmaElectronicaService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/FirmaElectronica";

        public FirmaElectronicaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<FirmaElectronica> FirmarFacturaAsync(int idFactura)
        {
            var response = await _http.PostAsync($"{BaseUrl}/firmar/{idFactura}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FirmaElectronica>() 
                ?? throw new Exception("Error al firmar la factura");
        }

        public async Task<bool> ValidarFirmaAsync(int idFactura)
        {
            var response = await _http.GetFromJsonAsync<ValidationResponse>($"{BaseUrl}/validar/{idFactura}");
            return response?.Valida ?? false;
        }

        public async Task<FirmaElectronica?> GetFirmaPorFacturaAsync(int idFactura)
        {
            try
            {
                return await _http.GetFromJsonAsync<FirmaElectronica>($"{BaseUrl}/{idFactura}");
            }
            catch
            {
                return null;
            }
        }

        private class ValidationResponse
        {
            public bool Valida { get; set; }
        }
    }
}
