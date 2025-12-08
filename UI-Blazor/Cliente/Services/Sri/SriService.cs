using System.Net.Http.Json;
using System.Text.Json;

namespace Cliente.Services.Sri
{
    public class SriService
    {
        private readonly HttpClient _http;

        public SriService(HttpClient http)
        {
            _http = http;
        }

        public async Task<SriRecepcionResponse> EnviarFacturaSriAsync(int idFactura)
        {
            var response = await _http.PostAsync($"api/sri/enviar/{idFactura}", null);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al enviar al SRI: {error}");
            }

            return await response.Content.ReadFromJsonAsync<SriRecepcionResponse>() ?? new SriRecepcionResponse();
        }

        public async Task<SriAutorizacionResult> AutorizarFacturaSriAsync(string claveAcceso)
        {
            var response = await _http.GetAsync($"api/sri/autorizar/{claveAcceso}");
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al autorizar en SRI: {error}");
            }

            return await response.Content.ReadFromJsonAsync<SriAutorizacionResult>() ?? new SriAutorizacionResult();
        }
    }

    public class SriRecepcionResponse
    {
        public string Stage { get; set; } = string.Empty;
        public SriRecepcionResult Response { get; set; } = new();
    }

    public class SriRecepcionResult
    {
        public string Estado { get; set; } = string.Empty;
        public string ClaveAcceso { get; set; } = string.Empty;
        public string Mensajes { get; set; } = string.Empty;
        // public string RawXml { get; set; } // Ignoramos el XML raw en cliente por ahora
    }

    public class SriAutorizacionResult
    {
        public string Estado { get; set; } = string.Empty;
        public string NumeroAutorizacion { get; set; } = string.Empty;
        public string FechaAutorizacion { get; set; } = string.Empty;
        public string XmlAutorizado { get; set; } = string.Empty;
        public string Mensajes { get; set; } = string.Empty;
        public string RawXml { get; set; } = string.Empty;
    }
}
