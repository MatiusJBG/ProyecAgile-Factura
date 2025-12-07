using Application.DTOs.Cliente;
using System.Net.Http.Json;

namespace Cliente.Services.Clientes
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> GetClientesAsync();          // Para tablas
        Task<List<ClienteDto>> SearchClientesAsync(string searchTerm = "");  // Para autocomplete
        Task<ClienteDto?> GetClienteByIdAsync(int id);
        Task<ClienteDto> CreateClienteAsync(ClienteDto cliente);
        Task UpdateClienteAsync(ClienteDto cliente);
        Task DeleteClienteAsync(int id);
    }

    public class ClienteService : IClienteService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/clientes";

        public ClienteService(HttpClient http) => _http = http;

        // Para tablas paginadas
        public async Task<List<ClienteDto>> GetClientesAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<ClienteDto>>(BaseUrl) ?? new();
            }
            catch
            {
                return new List<ClienteDto>();
            }
        }

        // Para autocomplete/búsquedas
        public async Task<List<ClienteDto>> SearchClientesAsync(string searchTerm = "")
        {
            try
            {
                var url = string.IsNullOrWhiteSpace(searchTerm) 
                    ? $"{BaseUrl}/search" 
                    : $"{BaseUrl}/search?q={Uri.EscapeDataString(searchTerm)}";
                return await _http.GetFromJsonAsync<List<ClienteDto>>(url) ?? new();
            }
            catch
            {
                return new List<ClienteDto>();
            }
        }

        public async Task<ClienteDto?> GetClienteByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<ClienteDto>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<ClienteDto> CreateClienteAsync(ClienteDto cliente)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(BaseUrl, cliente);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ClienteDto>() ?? cliente;
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error HTTP al crear cliente: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateClienteAsync(ClienteDto cliente)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{BaseUrl}/{cliente.Id_Cli}", cliente);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error HTTP al actualizar cliente {cliente.Id_Cli}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteClienteAsync(int id)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
