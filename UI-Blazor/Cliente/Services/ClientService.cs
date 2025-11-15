using UI_Blazor.Client.Models;

namespace UI_Blazor.Client.Services

{
    public interface IClientService
    {
        Task<List<Models.Cliente>> GetClientsAsync();
    }

    public class ClientService : IClientService
    {
        private List<Models.Cliente> mockClientes = new List<Models.Cliente>
        {
            new Models.Cliente { Id = 1001, Nombres = "Luis", Apellidos = "García", NombreComercial = "N/A", Tipo = TipoCliente.PersonaNatural, NumeroIdentificacion = "1710000001", Email = "luis@example.com", Telefono = "0991234567", UltimaFactura = new DateTime(2025, 10, 12), Saldo = 0, Estado = "Activo" },
            new Models.Cliente { Id = 1002, Nombres = "N/A", Apellidos = "N/A", NombreComercial = "Tienda Sol S.A.", Tipo = TipoCliente.Empresa, NumeroIdentificacion = "0990000000001", Email = "ventas@sol.com", Telefono = "0429876543", UltimaFactura = new DateTime(2025, 11, 05), Saldo = 120.50m, Estado = "Activo" },
            new Models.Cliente { Id = 1003, Nombres = "Carla", Apellidos = "Méndez", NombreComercial = "N/A", Tipo = TipoCliente.Extranjero, NumeroIdentificacion = "A1234567", Email = "carla.m@mail.com", Telefono = "0225555555", UltimaFactura = new DateTime(2025, 09, 28), Saldo = 450.75m, Estado = "Inactivo" },
            new Models.Cliente { Id = 1004, Nombres = "N/A", Apellidos = "N/A", NombreComercial = "Distribuciones XYZ", Tipo = TipoCliente.Empresa, NumeroIdentificacion = "1790000000001", Email = "xyz@dist.com", Telefono = "0987654321", UltimaFactura = new DateTime(2024, 01, 11), Saldo = 120.00m, Estado = "Activo" },
            new Models.Cliente { Id = 1005, Nombres = "Javier", Apellidos = "Pérez", NombreComercial = "N/A", Tipo = TipoCliente.PersonaNatural, NumeroIdentificacion = "1710000002", Email = "jpperz@example.com", Telefono = "0992345678", UltimaFactura = new DateTime(2024, 12, 01), Saldo = 0, Estado = "Inactivo" },
            new Models.Cliente { Id = 1006, Nombres = "N/A", Apellidos = "N/A", NombreComercial = "Global Tech Solutions", Tipo = TipoCliente.Empresa, NumeroIdentificacion = "0990000000002", Email = "info@global.com", Telefono = "0421234567", UltimaFactura = new DateTime(2024, 05, 20), Saldo = 750.00m, Estado = "Activo" },
            new Models.Cliente { Id = 1007, Nombres = "María", Apellidos = "Rodríguez", NombreComercial = "N/A", Tipo = TipoCliente.PersonaNatural, NumeroIdentificacion = "1710000003", Email = "maria.r@web.es", Telefono = "0993456789", UltimaFactura = new DateTime(2024, 03, 15), Saldo = 0, Estado = "Activo" },
            new Models.Cliente { Id = 1008, Nombres = "N/A", Apellidos = "N/A", NombreComercial = "Innovate Corp", Tipo = TipoCliente.Empresa, NumeroIdentificacion = "1790000000002", Email = "contact@innovate.net", Telefono = "0226666666", UltimaFactura = new DateTime(2023, 11, 01), Saldo = 2500.00m, Estado = "Activo" },
            new Models.Cliente { Id = 1009, Nombres = "Pedro", Apellidos = "Sánchez", NombreComercial = "N/A", Tipo = TipoCliente.Extranjero, NumeroIdentificacion = "P9876543", Email = "pedro.s@domain.com", Telefono = "0994567890", UltimaFactura = new DateTime(2024, 01, 30), Saldo = 0, Estado = "Inactivo" },
            new Models.Cliente { Id = 1010, Nombres = "Juana", Apellidos = "Vera", NombreComercial = "N/A", Tipo = TipoCliente.PersonaNatural, NumeroIdentificacion = "1710000004", Email = "juana.v@mail.com", Telefono = "0995678901", UltimaFactura = new DateTime(2024, 06, 10), Saldo = 150.00m, Estado = "Activo" }
        };

        public Task<List<Models.Cliente>> GetClientsAsync() => Task.FromResult(mockClientes);
    }
}