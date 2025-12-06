using Application.DTOs.Certificados;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace Cliente.Services.Certificados
{
    // Esta es una implementación HTTP Client - las interfaces están en Core.Interfaces
    public class CertificadoHttpClient
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/Certificados";

        public CertificadoHttpClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CertificadoDigitalDto>> GetAllCertificadosAsync()
        {
            return await _http.GetFromJsonAsync<List<CertificadoDigitalDto>>(BaseUrl) 
                ?? new List<CertificadoDigitalDto>();
        }

        public async Task<CertificadoDigitalDto?> GetCertificadoActivoAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<CertificadoDigitalDto>($"{BaseUrl}/activo");
            }
            catch
            {
                return null;
            }
        }

        public async Task<CertificadoDigitalDto> SubirCertificadoAsync(IBrowserFile archivo, string password, string nombre)
        {
            using var content = new MultipartFormDataContent();

            // Agregar archivo
            var fileContent = new StreamContent(archivo.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024)); // 10MB max
            content.Add(fileContent, "archivo", archivo.Name);

            // Agregar password
            content.Add(new StringContent(password), "password");

            // Agregar nombre
            content.Add(new StringContent(nombre), "nombre");

            var response = await _http.PostAsync(BaseUrl, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CertificadoDigitalDto>() 
                ?? throw new Exception("Error al subir el certificado");
        }

        public async Task<bool> ActivarCertificadoAsync(int idCertificado)
        {
            var response = await _http.PutAsync($"{BaseUrl}/{idCertificado}/activar", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ValidarCertificadoAsync(int idCertificado)
        {
            var response = await _http.GetFromJsonAsync<ValidationResponse>($"{BaseUrl}/{idCertificado}/validar");
            return response?.Valido ?? false;
        }

        public async Task EliminarCertificadoAsync(int idCertificado)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{idCertificado}");
            response.EnsureSuccessStatusCode();
        }

        private class ValidationResponse
        {
            public bool Valido { get; set; }
        }
    }
}
