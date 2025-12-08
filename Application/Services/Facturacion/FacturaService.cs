using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Factura;
using Application.DTOs.Producto;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Enums.Clientes; using Core.Enums.Facturacion;
using Core.Exceptions;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;

namespace Application.Services.Facturacion
{
    public class FacturaService : ServiceBase<Factura, FacturaDto>
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ILoteRepository _loteRepository;

        public FacturaService(
            IFacturaRepository facturaRepository,
            IClienteRepository clienteRepository,
            ILoteRepository loteRepository)
        {
            _facturaRepository = facturaRepository;
            _clienteRepository = clienteRepository;
            _loteRepository = loteRepository;
        }

        public async Task<IEnumerable<FacturaDto>> GetAllFacturasAsync()
        {
            var facturas = await _facturaRepository.GetFacturasWithClienteAsync();
            return facturas.Select(MapToDto);
        }

        public async Task<FacturaStatsDto> GetEstadisticasAsync()
        {
            var (totalCount, totalPagado) = await _facturaRepository.GetFacturaStatsAsync();
            return new FacturaStatsDto
            {
                TotalFacturas = totalCount,
                TotalCobrado = totalPagado
            };
        }

        public async Task<PagedResult<FacturaDto>> GetFacturasPagedAsync(int page, int pageSize, string searchTerm = "", string estado = "")
        {
            var (items, totalCount) = await _facturaRepository.GetFacturasPagedAsync(page, pageSize, searchTerm, estado);
            
            return new PagedResult<FacturaDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<FacturaDto?> GetFacturaByIdAsync(int id)
        {
            var factura = await _facturaRepository.GetFacturaWithDetailsAsync(id);
            return factura != null ? MapToDto(factura) : null;
        }

        public async Task<FacturaDto> CreateFacturaAsync(FacturaDto facturaDto)
        {
            // Validar que el cliente existe
            var cliente = await EntityValidator.GetOrThrowAsync<Cliente, IClienteRepository>(_clienteRepository, facturaDto.Id_Cli_Per, "Cliente");

            // Validar que existan detalles
            if (facturaDto.Detalles == null || !facturaDto.Detalles.Any() || facturaDto.Detalles.All(d => d.Id_Pro_Per == 0))
            {
                throw new BusinessValidationException("La factura debe tener al menos un producto válido.");
            }

            // Validar y actualizar stock de lotes
            foreach (var detalle in facturaDto.Detalles)
            {
                if (detalle.Id_Pro_Per == 0) continue; // Skip invalid lines if any (should act as filter or blocker) check above blocks all 0.
                var lote = await EntityValidator.GetOrThrowAsync<Lote, ILoteRepository>(_loteRepository, detalle.Id_Lote_Per, "Lote");

                if (lote.Cantidad_Disponible < detalle.Cantidad_Comprada)
                {
                    throw new BusinessValidationException(
                        $"Stock insuficiente en lote {lote.Id_Lote}. Disponible: {lote.Cantidad_Disponible}, Solicitado: {detalle.Cantidad_Comprada}");
                }

                // Reducir stock del lote
                lote.Cantidad_Disponible -= detalle.Cantidad_Comprada;
                await _loteRepository.UpdateAsync(lote);
            }

            var factura = MapToEntity(facturaDto);
            
            // Calcular totales
            decimal subtotal = factura.Detalles.Sum(d => d.Cantidad_Comprada * d.Precio_Venta_Unit);
            decimal iva = subtotal * 0.15m; // IVA del 15%
            
            factura.Tot_Fac_Sin_IVA = subtotal;
            factura.IVA_Fac = iva;
            factura.Tot_Fac_Con_IVA = subtotal + iva;

            var creada = await _facturaRepository.AddAsync(factura);
            
            // Recargar con relaciones
            var facturaCompleta = await _facturaRepository.GetFacturaWithDetailsAsync(creada.Id_Fac);
            return MapToDto(facturaCompleta!);
        }

        public async Task UpdateFacturaAsync(int id, FacturaDto facturaDto)
        {
            var factura = await GetEntityOrThrowAsync(_facturaRepository, id, "Factura");

            factura.Fec_Fac = facturaDto.Fec_Fac;
            factura.Tot_Fac_Sin_IVA = facturaDto.Tot_Fac_Sin_IVA;
            factura.IVA_Fac = facturaDto.IVA_Fac;
            factura.Tot_Fac_Con_IVA = facturaDto.Tot_Fac_Con_IVA;

            await _facturaRepository.UpdateAsync(factura);
        }

        public async Task DeleteFacturaAsync(int id)
        {
            var factura = await GetEntityOrThrowAsync(_facturaRepository, id, "Factura");

            // Restaurar stock de lotes
            foreach (var detalle in factura.Detalles)
            {
                var lote = await _loteRepository.GetByIdAsync(detalle.Id_Lote_Per);
                if (lote != null)
                {
                    lote.Cantidad_Disponible += detalle.Cantidad_Comprada;
                    await _loteRepository.UpdateAsync(lote);
                }
            }

            await _facturaRepository.DeleteAsync(factura);
        }

        protected override FacturaDto MapToDto(Factura factura)
        {
            return new FacturaDto
            {
                Id_Fac = factura.Id_Fac,
                Fec_Fac = factura.Fec_Fac,
                Id_Cli_Per = factura.Id_Cli_Per,
                Tot_Fac_Sin_IVA = factura.Tot_Fac_Sin_IVA ?? 0,
                IVA_Fac = factura.IVA_Fac ?? 0,
                Tot_Fac_Con_IVA = factura.Tot_Fac_Con_IVA ?? 0,
                
                Estado = factura.Estado,
                ClaveAcceso = factura.ClaveAcceso,
                MensajeError = factura.MensajeError,

                ClienteNombre = factura.Cliente.Tipo_Cliente == TipoCliente.EMPRESA
                    ? factura.Cliente.Nombre
                    : $"{factura.Cliente.Nombre} {factura.Cliente.Apellido}".Trim(),
                Detalles = factura.Detalles.Select(d => new DetalleFacturaDto
                {
                    Id_Det_Fac = d.Id_Det_Fac,
                    Id_Fac_Per = d.Id_Fac_Per,
                    Id_Lote_Per = d.Id_Lote_Per,
                    Id_Pro_Per = d.Id_Pro_Per,
                    Cantidad_Comprada = d.Cantidad_Comprada,
                    Precio_Venta_Unit = d.Precio_Venta_Unit,
                    ProductoNombre = d.Producto.Nom_Pro
                }).ToList()
            };
        }

        protected override Factura MapToEntity(FacturaDto dto)
        {
            return new Factura
            {
                Fec_Fac = dto.Fec_Fac,
                Id_Cli_Per = dto.Id_Cli_Per,
                ClaveAcceso = dto.ClaveAcceso,
                MensajeError = dto.MensajeError,
                Detalles = dto.Detalles.Select(d => new DetalleFactura
                {
                    Id_Lote_Per = d.Id_Lote_Per,
                    Id_Pro_Per = d.Id_Pro_Per,
                    Cantidad_Comprada = d.Cantidad_Comprada,
                    Precio_Venta_Unit = d.Precio_Venta_Unit
                }).ToList()
            };
        }
    }
}
