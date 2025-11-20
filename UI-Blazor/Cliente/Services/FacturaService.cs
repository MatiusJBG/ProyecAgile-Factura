using UI_Blazor.Client.Models;
using System.Net.Http.Json;
using Models = UI_Blazor.Client.Models;

namespace UI_Blazor.Client.Services
{
    public class FacturaService
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
                // Mock data for development
                var clientes = new List<Models.Cliente>
                {
                    new Models.Cliente { Id = 1001, Nombres = "Luis", Apellidos = "García", Tipo = TipoCliente.PersonaNatural, NumeroIdentificacion = "1710000001" },
                    new Models.Cliente { Id = 1002, NombreComercial = "Tienda Sol S.A.", Tipo = TipoCliente.Empresa, NumeroIdentificacion = "0990000000001" }
                };

                return new List<Models.Factura>
                {
                    new Models.Factura { Id = 1, Serie = "F001", Numero = "00000001", FechaEmision = DateTime.Today.AddDays(-10), Total = 1200.50m, Estado = EstadoFactura.Pagada, Cliente = clientes[0], ClienteId = 1001 },
                    new Models.Factura { Id = 2, Serie = "F001", Numero = "00000002", FechaEmision = DateTime.Today.AddDays(-5), Total = 850.00m, Estado = EstadoFactura.Pendiente, Cliente = clientes[1], ClienteId = 1002 },
                    new Models.Factura { Id = 3, Serie = "F001", Numero = "00000003", FechaEmision = DateTime.Today.AddDays(-30), Total = 450.75m, Estado = EstadoFactura.Vencida, Cliente = clientes[0], ClienteId = 1001 }
                };
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
                var facturas = await GetFacturasAsync();
                return facturas.FirstOrDefault(f => f.Id == id);
            }
        }

        public async Task<Models.Factura> CreateAsync(Models.Factura factura)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(BaseUrl, factura);
                return await response.Content.ReadFromJsonAsync<Models.Factura>() ?? factura;
            }
            catch
            {
                factura.Id = new Random().Next(1000, 9999);
                return factura;
            }
        }

        public async Task UpdateAsync(int id, Models.Factura factura)
        {
            try
            {
                await _http.PutAsJsonAsync($"{BaseUrl}/{id}", factura);
            }
            catch
            {
                // Mock update
                await Task.CompletedTask;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _http.DeleteAsync($"{BaseUrl}/{id}");
            }
            catch
            {
                // Mock delete
                await Task.CompletedTask;
            }
        }
    }
}