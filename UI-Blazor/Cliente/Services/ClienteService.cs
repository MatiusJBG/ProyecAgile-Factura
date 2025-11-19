using UI_Blazor.Client.Models;
using Models = UI_Blazor.Client.Models;

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
        private readonly IClientService _clientService;

        public ClienteService(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<List<Models.Cliente>> GetClientesAsync()
        {
            return await _clientService.GetClientsAsync();
        }

        public async Task<Models.Cliente?> GetClienteByIdAsync(int id)
        {
            var clientes = await GetClientesAsync();
            return clientes.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Models.Cliente> CreateClienteAsync(Models.Cliente cliente)
        {
            // Mock implementation - in real app would call API
            await Task.CompletedTask;
            return cliente;
        }

        public async Task UpdateClienteAsync(Models.Cliente cliente)
        {
            // Mock implementation - in real app would call API
            await Task.CompletedTask;
        }

        public async Task DeleteClienteAsync(int id)
        {
            // Mock implementation - in real app would call API
            await Task.CompletedTask;
        }
    }
}

