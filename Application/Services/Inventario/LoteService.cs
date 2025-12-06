using Application.Common;
using Application.Common.Validators;
using Application.DTOs.Producto;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Exceptions;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;

namespace Application.Services.Inventario
{
    public class LoteService : ServiceBase<Lote, LoteDto>
    {
        private readonly ILoteRepository _loteRepository;
        private readonly IProductoRepository _productoRepository;

        public LoteService(ILoteRepository loteRepository, IProductoRepository productoRepository)
        {
            _loteRepository = loteRepository;
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<LoteDto>> GetAllLotesAsync()
        {
            var lotes = await _loteRepository.GetAllAsync();
            return lotes.Select(MapToDto);
        }

        public async Task<LoteDto?> GetLoteByIdAsync(int id)
        {
            var lote = await _loteRepository.GetByIdAsync(id);
            return lote != null ? MapToDto(lote) : null;
        }

        public async Task<IEnumerable<LoteDto>> GetLotesByProductoAsync(int idProducto)
        {
            var lotes = await _loteRepository.GetLotesByProductoAsync(idProducto);
            return lotes.Select(MapToDto);
        }

        public async Task<IEnumerable<LoteDto>> GetLotesDisponiblesAsync()
        {
            var lotes = await _loteRepository.GetLotesDisponiblesAsync();
            return lotes.Select(MapToDto);
        }

        public async Task<LoteDto> CreateLoteAsync(LoteDto loteDto)
        {
            // Validar que el producto existe
            var producto = await EntityValidator.GetOrThrowAsync<Producto, IProductoRepository>(_productoRepository, loteDto.Id_Pro_Per, "Producto");

            // Validar fechas
            DateValidator.ValidateExpirationAfterEntry(loteDto.Fec_Ent, loteDto.Fec_Exp, "Lote");

            var lote = MapToEntity(loteDto);
            var creado = await _loteRepository.AddAsync(lote);
            return MapToDto(creado);
        }

        public async Task UpdateLoteAsync(int id, LoteDto loteDto)
        {
            var lote = await GetEntityOrThrowAsync(_loteRepository, id, "Lote");

            // Validar fechas
            DateValidator.ValidateExpirationAfterEntry(loteDto.Fec_Ent, loteDto.Fec_Exp, "Lote");

            lote.Fec_Ent = loteDto.Fec_Ent;
            lote.Fec_Exp = loteDto.Fec_Exp;
            lote.Cantidad_Recibida = loteDto.Cantidad_Recibida;
            lote.Cantidad_Disponible = loteDto.Cantidad_Disponible;
            lote.Precio_Unitario = loteDto.Precio_Unitario;

            await _loteRepository.UpdateAsync(lote);
        }

        public async Task DeleteLoteAsync(int id)
        {
            var lote = await GetEntityOrThrowAsync(_loteRepository, id, "Lote");

            await _loteRepository.DeleteAsync(lote);
        }

        protected override LoteDto MapToDto(Lote lote)
        {
            return new LoteDto
            {
                Id_Lote = lote.Id_Lote,
                Id_Pro_Per = lote.Id_Pro_Per,
                Fec_Ent = lote.Fec_Ent,
                Fec_Exp = lote.Fec_Exp,
                Cantidad_Recibida = lote.Cantidad_Recibida,
                Cantidad_Disponible = lote.Cantidad_Disponible,
                Precio_Unitario = lote.Precio_Unitario
            };
        }

        protected override Lote MapToEntity(LoteDto dto)
        {
            return new Lote
            {
                Id_Pro_Per = dto.Id_Pro_Per,
                Fec_Ent = dto.Fec_Ent,
                Fec_Exp = dto.Fec_Exp,
                Cantidad_Recibida = dto.Cantidad_Recibida,
                Cantidad_Disponible = dto.Cantidad_Disponible,
                Precio_Unitario = dto.Precio_Unitario
            };
        }
    }
}
