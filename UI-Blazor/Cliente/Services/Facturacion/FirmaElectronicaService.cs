using System.Net.Http.Json;
using Application.DTOs.Certificados;

namespace Cliente.Services.Facturacion
{
    // Esta es una implementación HTTP Client - las interfaces están en Core.Interfaces
    public class FirmaElectronicaHttpClient
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/FirmaElectronica";

        public FirmaElectronicaHttpClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<FirmaElectronicaDto> FirmarFacturaAsync(int idFactura)
        {
            var response = await _http.PostAsync($"{BaseUrl}/firmar/{idFactura}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FirmaElectronicaDto>() 
                ?? throw new Exception("Error al firmar la factura");
        }

        public async Task<bool> ValidarFirmaAsync(int idFactura)
        {
            var response = await _http.GetFromJsonAsync<ValidationResponse>($"{BaseUrl}/validar/{idFactura}");
            return response?.Valida ?? false;
        }

        public async Task<FirmaElectronicaDto?> GetFirmaPorFacturaAsync(int idFactura)
        {
            try
            {
                return await _http.GetFromJsonAsync<FirmaElectronicaDto>($"{BaseUrl}/{idFactura}");
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
