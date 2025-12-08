using Application.Common;
using Application.DTOs.Cliente;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Enums.Clientes; using Core.Enums.Facturacion;
using Core.Exceptions;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Application.Interfaces;

namespace Application.Services.Clientes
{
    public class ClienteService : ServiceBase<Cliente, ClienteDto>
    {
        private readonly IClienteRepository _clienteRepository;

        private readonly IFileCacheService _fileCache;

        public ClienteService(IClienteRepository clienteRepository, IFileCacheService fileCache)
        {
            _clienteRepository = clienteRepository;
            _fileCache = fileCache;
        }

        // Para tablas paginadas - SIEMPRE desde BD, sin caché
        public async Task<IEnumerable<ClienteDto>> GetAllClientesAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            return clientes.Select(MapToDto);
        }

        // Para búsquedas/autocomplete - USA caché para performance
        public async Task<IEnumerable<ClienteDto>> SearchClientesAsync(string searchTerm = "")
        {
            var cached = await _fileCache.GetClientesCacheAsync();
            
            if (!cached.Any())
            {
                var fromDb = await _clienteRepository.GetAllAsync();
                cached = fromDb.Select(MapToDto).ToList();
                await _fileCache.SaveClientesCacheAsync(cached);
            }
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLowerInvariant();
                return cached.Where(c => 
                    c.Nombre.ToLowerInvariant().Contains(searchTerm) ||
                    c.Apellido.ToLowerInvariant().Contains(searchTerm) ||
                    c.Num_Documento.Contains(searchTerm) ||
                    c.Correo.ToLowerInvariant().Contains(searchTerm));
            }
            
            return cached;
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
                throw new DuplicateEntityException($"Ya existe un cliente con el documento {clienteDto.Num_Documento}");
            }

            var cliente = MapToEntity(clienteDto);
            var creado = await _clienteRepository.AddAsync(cliente);
            return MapToDto(creado);
        }

        public async Task UpdateClienteAsync(int id, ClienteDto clienteDto)
        {
            var cliente = await GetEntityOrThrowAsync(_clienteRepository, id, "Cliente");

            // Validar que no exista otro cliente con el mismo documento
            var existente = await _clienteRepository.GetByDocumentoAsync(clienteDto.Num_Documento);
            if (existente != null && existente.Id_Cli != id)
            {
                throw new DuplicateEntityException($"Ya existe otro cliente con el documento {clienteDto.Num_Documento}");
            }

            cliente.Tipo_Cliente = clienteDto.Tipo_Cliente;
            cliente.Tipo_Documento = clienteDto.Tipo_Documento;
            cliente.Num_Documento = clienteDto.Num_Documento;
            cliente.Nombre = clienteDto.Nombre;
            cliente.Apellido = clienteDto.Apellido;
            cliente.Direccion = clienteDto.Direccion;
            cliente.Correo = clienteDto.Correo;
            cliente.Telefono = clienteDto.Telefono;
            cliente.Activo = clienteDto.Activo;

            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task DeleteClienteAsync(int id)
        {
            var cliente = await GetEntityOrThrowAsync(_clienteRepository, id, "Cliente");

            await _clienteRepository.DeleteAsync(cliente);
        }

        protected override ClienteDto MapToDto(Cliente cliente)
        {
            return new ClienteDto
            {
                Id_Cli = cliente.Id_Cli,
                Tipo_Cliente = cliente.Tipo_Cliente,
                Tipo_Documento = cliente.Tipo_Documento,
                Num_Documento = cliente.Num_Documento,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido ?? string.Empty,
                Direccion = cliente.Direccion ?? string.Empty,
                Correo = cliente.Correo ?? string.Empty,
                Telefono = cliente.Telefono ?? string.Empty,
                Activo = cliente.Activo
            };
        }

        protected override Cliente MapToEntity(ClienteDto dto)
        {
            return new Cliente
            {
                Tipo_Cliente = dto.Tipo_Cliente,
                Tipo_Documento = dto.Tipo_Documento,
                Num_Documento = dto.Num_Documento,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Direccion = dto.Direccion,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Activo = dto.Activo
            };
        }
    }
}
