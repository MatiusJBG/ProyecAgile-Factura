using Application.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class ProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ILoteRepository _loteRepository;
        private readonly IPrecioRepository _precioRepository;

        public ProductoService(IProductoRepository productoRepository, ILoteRepository loteRepository, IPrecioRepository precioRepository)
        {
            _productoRepository = productoRepository;
            _loteRepository = loteRepository;
            _precioRepository = precioRepository;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllProductosAsync()
        {
            var productos = await _productoRepository.GetProductosWithStockAsync();
            var dtos = new List<ProductoDto>();
            
            foreach (var producto in productos)
            {
                var precio = await _precioRepository.GetCurrentPriceAsync(producto.Id_Pro);
                dtos.Add(MapToDto(producto, precio));
            }
            
            return dtos;
        }

        public async Task<ProductoDto?> GetProductoByIdAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null) return null;
            
            var precio = await _precioRepository.GetCurrentPriceAsync(id);
            return MapToDto(producto, precio);
        }

        public async Task<ProductoDto> CreateProductoAsync(ProductoConLoteDto productoDto)
        {
            var producto = MapToEntity(productoDto);
            var creado = await _productoRepository.AddAsync(producto);

            // Si viene información de lote, crearlo
            if (productoDto.Cantidad_Recibida.HasValue && productoDto.Precio_Unitario.HasValue)
            {
                var lote = new Lote
                {
                    Id_Pro_Per = creado.Id_Pro,
                    Fec_Ent = productoDto.Fec_Ent ?? DateTime.Now,
                    Fec_Exp = productoDto.Fec_Exp ?? DateTime.Now.AddMonths(1),
                    Cantidad_Recibida = productoDto.Cantidad_Recibida.Value,
                    Cantidad_Disponible = productoDto.Cantidad_Disponible ?? productoDto.Cantidad_Recibida.Value,
                    Precio_Unitario = productoDto.Precio_Unitario.Value,
                    // Precio_Lote se calcula en base de datos o podemos calcularlo aquí si es necesario
                    Precio_Lote = productoDto.Cantidad_Recibida.Value * productoDto.Precio_Unitario.Value
                };
                
                await _loteRepository.AddAsync(lote);
            }

            // Si viene precio de venta, crearlo
            if (productoDto.Precio_Venta.HasValue)
            {
                var precio = new Precio
                {
                    Id_Pro_Per = creado.Id_Pro,
                    Precio_Venta = productoDto.Precio_Venta.Value,
                    Fecha_Actualizacion = DateTime.Now,
                    Motivo = "Precio Inicial"
                };
                await _precioRepository.AddAsync(precio);
            }

            // Recargar producto con sus lotes para devolver el DTO completo
            var productoCompleto = await _productoRepository.GetByIdAsync(creado.Id_Pro);
            
            // Obtener precio actual
            var precioActual = await _precioRepository.GetCurrentPriceAsync(creado.Id_Pro);
            
            return MapToDto(productoCompleto!, precioActual);
        }

        public async Task UpdateProductoAsync(int id, ProductoDto productoDto)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null)
            {
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
            }

            producto.Tip_Pro = productoDto.Tip_Pro;
            producto.Nom_Pro = productoDto.Nom_Pro;
            producto.Marca = productoDto.Marca;

            await _productoRepository.UpdateAsync(producto);
        }

        public async Task DeleteProductoAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null)
            {
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
            }

            await _productoRepository.DeleteAsync(producto);
        }

        private ProductoDto MapToDto(Producto producto, Precio? precio = null)
        {
            // Ordenar lotes por fecha de entrada (FIFO)
            var lotesOrdenados = producto.Lotes
                .OrderBy(l => l.Fec_Ent)
                .Select(l => new LoteDto
                {
                    Id_Lote = l.Id_Lote,
                    Id_Pro_Per = l.Id_Pro_Per,
                    Fec_Ent = l.Fec_Ent,
                    Fec_Exp = l.Fec_Exp,
                    Cantidad_Recibida = l.Cantidad_Recibida,
                    Cantidad_Disponible = l.Cantidad_Disponible,
                    Precio_Unitario = l.Precio_Unitario,
                    Precio_Lote = l.Precio_Lote
                })
                .ToList();

            return new ProductoDto
            {
                Id_Pro = producto.Id_Pro,
                Tip_Pro = producto.Tip_Pro,
                Nom_Pro = producto.Nom_Pro,
                Marca = producto.Marca,
                StockTotal = producto.Lotes.Sum(l => l.Cantidad_Disponible),
                NumLotes = producto.Lotes.Count,
                Lotes = lotesOrdenados,
                Precio_Venta = precio?.Precio_Venta ?? 0
            };
        }

        private Producto MapToEntity(ProductoConLoteDto dto)
        {
            return new Producto
            {
                Tip_Pro = dto.Tip_Pro,
                Nom_Pro = dto.Nom_Pro,
                Marca = dto.Marca
            };
        }
    }
}
