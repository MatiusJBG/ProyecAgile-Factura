using Application.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class ProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllProductosAsync()
        {
            var productos = await _productoRepository.GetProductosWithStockAsync();
            return productos.Select(MapToDto);
        }

        public async Task<ProductoDto?> GetProductoByIdAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            return producto != null ? MapToDto(producto) : null;
        }

        public async Task<ProductoDto> CreateProductoAsync(ProductoDto productoDto)
        {
            var producto = MapToEntity(productoDto);
            var creado = await _productoRepository.AddAsync(producto);
            return MapToDto(creado);
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

        private ProductoDto MapToDto(Producto producto)
        {
            return new ProductoDto
            {
                Id_Pro = producto.Id_Pro,
                Tip_Pro = producto.Tip_Pro,
                Nom_Pro = producto.Nom_Pro,
                Marca = producto.Marca,
                StockTotal = producto.Lotes.Sum(l => l.Cantidad_Disponible),
                NumLotes = producto.Lotes.Count
            };
        }

        private Producto MapToEntity(ProductoDto dto)
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
