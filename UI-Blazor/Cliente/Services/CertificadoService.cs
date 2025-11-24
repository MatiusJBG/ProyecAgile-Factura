using UI_Blazor.Client.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
namespace UI_Blazor.Client.Services
{
    public interface ICertificadoService
    {
        Task<List<CertificadoDigital>> GetAllCertificadosAsync();
        Task<CertificadoDigital?> GetCertificadoActivoAsync();
        Task<CertificadoDigital> SubirCertificadoAsync(IBrowserFile archivo, string password, string nombre);
        Task<bool> ActivarCertificadoAsync(int idCertificado);
        Task<bool> ValidarCertificadoAsync(int idCertificado);
        Task EliminarCertificadoAsync(int idCertificado);
    }

    public class CertificadoService : ICertificadoService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/Certificados";

        public CertificadoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CertificadoDigital>> GetAllCertificadosAsync()
        {
            return await _http.GetFromJsonAsync<List<CertificadoDigital>>(BaseUrl) 
                ?? new List<CertificadoDigital>();
        }

        public async Task<CertificadoDigital?> GetCertificadoActivoAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<CertificadoDigital>($"{BaseUrl}/activo");
            }
            catch
            {
                return null;
            }
        }

        public async Task<CertificadoDigital> SubirCertificadoAsync(IBrowserFile archivo, string password, string nombre)
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

            return await response.Content.ReadFromJsonAsync<CertificadoDigital>() 
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
