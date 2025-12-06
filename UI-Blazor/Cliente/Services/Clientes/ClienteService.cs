using Application.DTOs.Cliente;
using System.Net.Http.Json;

namespace Cliente.Services.Clientes
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> GetClientesAsync();
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

        public async Task<List<ClienteDto>> GetClientesAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<ClienteDto>>(BaseUrl) ?? new();
            }
            catch
            {
                // Retorna lista vacía si backend no disponible (para desarrollo)
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
            var response = await _http.PostAsJsonAsync(BaseUrl, cliente);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClienteDto>() ?? cliente;
        }

        public async Task UpdateClienteAsync(ClienteDto cliente)
        {
            var response = await _http.PutAsJsonAsync($"{BaseUrl}/{cliente.Id_Cli}", cliente);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteClienteAsync(int id)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
