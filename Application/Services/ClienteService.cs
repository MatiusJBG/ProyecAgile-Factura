using Application.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<ClienteDto>> GetAllClientesAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            return clientes.Select(MapToDto);
        }

        public async Task<ClienteDto?> GetClienteByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            return cliente != null ? MapToDto(cliente) : null;
        }

        public async Task<ClienteDto> CreateClienteAsync(ClienteDto clienteDto)
        {
            // Validar que no exista el documento
            var existente = await _clienteRepository.GetByDocumentoAsync(clienteDto.Num_Documento);
            if (existente != null)
            {
                throw new InvalidOperationException($"Ya existe un cliente con el documento {clienteDto.Num_Documento}");
            }

            var cliente = MapToEntity(clienteDto);
            var creado = await _clienteRepository.AddAsync(cliente);
            return MapToDto(creado);
        }

        public async Task UpdateClienteAsync(int id, ClienteDto clienteDto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");
            }

            // Validar que no exista otro cliente con el mismo documento
            var existente = await _clienteRepository.GetByDocumentoAsync(clienteDto.Num_Documento);
            if (existente != null && existente.Id_Cli != id)
            {
                throw new InvalidOperationException($"Ya existe otro cliente con el documento {clienteDto.Num_Documento}");
            }

            cliente.Tipo_Cliente = (Core.Entities.TipoCliente)clienteDto.Tipo_Cliente;
            cliente.Tipo_Documento = (Core.Entities.TipoDocumento)clienteDto.Tipo_Documento;
            cliente.Num_Documento = clienteDto.Num_Documento;
            cliente.Nombre = clienteDto.Nombre;
            cliente.Apellido = clienteDto.Apellido;
            cliente.Direccion = clienteDto.Direccion;
            cliente.Correo = clienteDto.Correo;
            cliente.Telefono = clienteDto.Telefono;

            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task DeleteClienteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");
            }

            await _clienteRepository.DeleteAsync(cliente);
        }

        private ClienteDto MapToDto(Cliente cliente)
        {
            return new ClienteDto
            {
                Id_Cli = cliente.Id_Cli,
                Tipo_Cliente = (DTOs.TipoCliente)cliente.Tipo_Cliente,
                Tipo_Documento = (DTOs.TipoDocumento)cliente.Tipo_Documento,
                Num_Documento = cliente.Num_Documento,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido ?? string.Empty,
                Direccion = cliente.Direccion ?? string.Empty,
                Correo = cliente.Correo ?? string.Empty,
                Telefono = cliente.Telefono ?? string.Empty
            };
        }

        private Cliente MapToEntity(ClienteDto dto)
        {
            return new Cliente
            {
                Tipo_Cliente = (Core.Entities.TipoCliente)dto.Tipo_Cliente,
                Tipo_Documento = (Core.Entities.TipoDocumento)dto.Tipo_Documento,
                Num_Documento = dto.Num_Documento,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Direccion = dto.Direccion,
                Correo = dto.Correo,
                Telefono = dto.Telefono
            };
        }
    }
}
