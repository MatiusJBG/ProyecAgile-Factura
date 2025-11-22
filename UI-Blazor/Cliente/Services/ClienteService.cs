using Models = UI_Blazor.Client.Models;
using System.Net.Http.Json;

namespace UI_Blazor.Client.Services
{
    public interface IClienteService
    {
        Task<List<Models.Cliente>> GetClientesAsync();
        Task<Models.Cliente?> GetClienteByIdAsync(int id);
        Task<Models.Cliente> CreateClienteAsync(Models.Cliente cliente);
        Task UpdateClienteAsync(Models.Cliente cliente);
        Task DeleteClienteAsync(int id);
    }

    public class ClienteService : IClienteService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/clientes";

        public ClienteService(HttpClient http) => _http = http;

        public async Task<List<Models.Cliente>> GetClientesAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Models.Cliente>>(BaseUrl) ?? new();
            }
            catch
            {
                // Retorna lista vacía si backend no disponible (para desarrollo)
                return new List<Models.Cliente>();
            }
        }

        public async Task<Models.Cliente?> GetClienteByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Models.Cliente>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.Cliente> CreateClienteAsync(Models.Cliente cliente)
        {
            var response = await _http.PostAsJsonAsync(BaseUrl, cliente);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Models.Cliente>() ?? cliente;
        }

        public async Task UpdateClienteAsync(Models.Cliente cliente)
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
