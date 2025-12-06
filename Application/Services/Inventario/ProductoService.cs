using Application.Common;
using Application.DTOs.Producto;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Exceptions;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Application.Interfaces;

namespace Application.Services.Inventario
{
    public class ProductoService : ServiceBase<Producto, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ILoteRepository _loteRepository;
        private readonly IPrecioRepository _precioRepository;

        private readonly IFileCacheService _fileCache;

        public ProductoService(IProductoRepository productoRepository, ILoteRepository loteRepository, IPrecioRepository precioRepository, IFileCacheService fileCache)
        {
            _productoRepository = productoRepository;
            _loteRepository = loteRepository;
            _precioRepository = precioRepository;
            _fileCache = fileCache;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllProductosAsync()
        {
            // Try cache first
            var cached = await _fileCache.GetProductosCacheAsync();
            if (cached.Any())
            {
                return cached;
            }

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

        public async Task<ProductoDto?> GetProductoByNombreAsync(string nombre)
        {
            var producto = await _productoRepository.GetByNombreAsync(nombre);
            if (producto == null) return null;

            var precio = await _precioRepository.GetCurrentPriceAsync(producto.Id_Pro);
            return MapToDto(producto, precio);
        }

        public async Task<ProductoDto> CreateProductoAsync(ProductoConLoteDto productoDto)
        {
            // PASO 1: Verificar si ya existe un producto con el mismo nombre
            var productoExistente = await _productoRepository.GetByNombreAsync(productoDto.Nom_Pro);
            
            Producto producto;
            bool esProductoNuevo = productoExistente == null;

            if (productoExistente != null)
            {
                // El producto ya existe, vamos a agregar un nuevo lote
                producto = productoExistente;
                
                // Actualizar información básica si cambió (tipo, marca, imagen)
                bool productoModificado = false;
                
                if (!string.IsNullOrEmpty(productoDto.Tip_Pro) && producto.Tip_Pro != productoDto.Tip_Pro)
                {
                    producto.Tip_Pro = productoDto.Tip_Pro;
                    productoModificado = true;
                }
                
                if (!string.IsNullOrEmpty(productoDto.Marca) && producto.Marca != productoDto.Marca)
                {
                    producto.Marca = productoDto.Marca;
                    productoModificado = true;
                }
                
                if (!string.IsNullOrEmpty(productoDto.Imagen) && producto.Imagen != productoDto.Imagen)
                {
                    producto.Imagen = productoDto.Imagen;
                    productoModificado = true;
                }
                
                if (productoModificado)
                {
                    await _productoRepository.UpdateAsync(producto);
                }
            }
            else
            {
                // Es un producto nuevo, crearlo
                producto = MapToEntity(productoDto);
                producto = await _productoRepository.AddAsync(producto);
            }

            // PASO 2: Agregar el nuevo lote con su propia fecha de entrada
            if (productoDto.Cantidad_Recibida.HasValue && productoDto.Precio_Unitario.HasValue)
            {
                var nuevoLote = new Lote
                {
                    Id_Pro_Per = producto.Id_Pro,
                    Fec_Ent = productoDto.Fec_Ent ?? DateTime.Now, // Mantener la fecha del nuevo lote
                    Fec_Exp = productoDto.Fec_Exp ?? DateTime.Now.AddMonths(1),
                    Cantidad_Recibida = productoDto.Cantidad_Recibida.Value,
                    Cantidad_Disponible = productoDto.Cantidad_Disponible ?? productoDto.Cantidad_Recibida.Value,
                    Precio_Unitario = productoDto.Precio_Unitario.Value,
                    Precio_Lote = productoDto.Cantidad_Recibida.Value * productoDto.Precio_Unitario.Value
                };
                
                await _loteRepository.AddAsync(nuevoLote);
            }

            // PASO 3: Actualizar o crear precio de venta
            if (productoDto.Precio_Venta.HasValue)
            {
                // Validación: PVP no puede ser menor al Costo Unitario
                if (productoDto.Precio_Unitario.HasValue && productoDto.Precio_Venta.Value < productoDto.Precio_Unitario.Value)
                {
                    throw new ArgumentException($"El Precio de Venta (${productoDto.Precio_Venta.Value:N2}) no puede ser menor al Costo Unitario (${productoDto.Precio_Unitario.Value:N2})");
                }

                // Obtener precio actual
                var precioActual = await _precioRepository.GetCurrentPriceAsync(producto.Id_Pro);
                
                // Solo crear nuevo registro de precio si es diferente al actual
                if (precioActual == null || precioActual.Precio_Venta != productoDto.Precio_Venta.Value)
                {
                    var precio = new Precio
                    {
                        Id_Pro_Per = producto.Id_Pro,
                        Precio_Venta = productoDto.Precio_Venta.Value,
                        Fecha_Actualizacion = DateTime.Now,
                        Motivo = esProductoNuevo ? "Precio Inicial" : "Actualización por Nuevo Lote"
                    };
                    await _precioRepository.AddAsync(precio);
                }
            }

            // PASO 4: Recargar producto con sus lotes ordenados por FIFO
            var productoCompleto = await _productoRepository.GetByIdAsync(producto.Id_Pro);
            
            // Obtener precio actual
            var precioFinal = await _precioRepository.GetCurrentPriceAsync(producto.Id_Pro);
            
            return MapToDto(productoCompleto!, precioFinal);
        }

        public async Task UpdateProductoAsync(int id, ProductoDto productoDto)
        {
            var producto = await GetEntityOrThrowAsync(_productoRepository, id, "Producto");

            // 1. Actualizar datos básicos del producto
            producto.Tip_Pro = productoDto.Tip_Pro;
            producto.Nom_Pro = productoDto.Nom_Pro;
            producto.Marca = productoDto.Marca;
            producto.Imagen = productoDto.Imagen;

            await _productoRepository.UpdateAsync(producto);

            // 2. Actualizar datos del Lote (si existe alguno, actualizamos el primero/más antiguo por defecto)
            // NOTA: Esto asume que el usuario quiere editar el "lote inicial" o el primer lote disponible.
            var primerLote = producto.Lotes.OrderBy(l => l.Fec_Ent).FirstOrDefault();
            if (primerLote != null)
            {
                bool loteModificado = false;

                if (productoDto.Fec_Ent.HasValue) 
                {
                    primerLote.Fec_Ent = productoDto.Fec_Ent.Value;
                    loteModificado = true;
                }
                
                if (productoDto.Fec_Exp.HasValue)
                {
                    primerLote.Fec_Exp = productoDto.Fec_Exp.Value;
                    loteModificado = true;
                }

                if (productoDto.Cantidad_Recibida.HasValue)
                {
                    primerLote.Cantidad_Recibida = productoDto.Cantidad_Recibida.Value;
                    loteModificado = true;
                }

                if (productoDto.Cantidad_Disponible.HasValue)
                {
                    primerLote.Cantidad_Disponible = productoDto.Cantidad_Disponible.Value;
                    loteModificado = true;
                }

                if (productoDto.Precio_Unitario.HasValue)
                {
                    primerLote.Precio_Unitario = productoDto.Precio_Unitario.Value;
                    loteModificado = true;
                }

                if (loteModificado)
                {
                    // Recalcular precio total del lote
                    primerLote.Precio_Lote = primerLote.Cantidad_Recibida * primerLote.Precio_Unitario;
                    await _loteRepository.UpdateAsync(primerLote);
                }
            }

            // 3. Actualizar Precio de Venta (si cambió)
            // Como Precio_Venta es decimal (no nullable), siempre tiene valor.
            // Comparamos con el precio actual en BD.
            var precioActual = await _precioRepository.GetCurrentPriceAsync(id);
            
            // Si no hay precio o el precio es diferente, crear nuevo registro
            if (precioActual == null || precioActual.Precio_Venta != productoDto.Precio_Venta)
            {
                // Validación: PVP no puede ser menor al Costo Unitario
                // Usamos el precio unitario del DTO si viene, o el del primer lote si no viene en el DTO pero existe en BD
                decimal? costoUnitario = productoDto.Precio_Unitario;
                if (!costoUnitario.HasValue && primerLote != null)
                {
                    costoUnitario = primerLote.Precio_Unitario;
                }

                if (costoUnitario.HasValue && productoDto.Precio_Venta < costoUnitario.Value)
                {
                    throw new ArgumentException($"El Precio de Venta (${productoDto.Precio_Venta:N2}) no puede ser menor al Costo Unitario (${costoUnitario.Value:N2})");
                }

                var nuevoPrecio = new Precio
                {
                    Id_Pro_Per = id,
                    Precio_Venta = productoDto.Precio_Venta,
                    Fecha_Actualizacion = DateTime.Now,
                    Motivo = "Actualización de Precio"
                };
                await _precioRepository.AddAsync(nuevoPrecio);
            }
        }

        public async Task AddLoteAsync(int productoId, LoteDto loteDto)
        {
            var producto = await GetEntityOrThrowAsync(_productoRepository, productoId, "Producto");

            var nuevoLote = new Lote
            {
                Id_Pro_Per = productoId,
                Fec_Ent = loteDto.Fec_Ent,
                Fec_Exp = loteDto.Fec_Exp,
                Cantidad_Recibida = loteDto.Cantidad_Recibida,
                Cantidad_Disponible = loteDto.Cantidad_Disponible > 0 ? loteDto.Cantidad_Disponible : loteDto.Cantidad_Recibida,
                Precio_Unitario = loteDto.Precio_Unitario,
                Precio_Lote = loteDto.Cantidad_Recibida * loteDto.Precio_Unitario
            };

            await _loteRepository.AddAsync(nuevoLote);

            // Actualizar precio de venta si es necesario (opcional, pero buena práctica verificar)
            // Aquí podríamos implementar lógica para actualizar el precio del producto si el nuevo lote tiene un costo mayor
            // Por ahora, mantenemos el precio existente a menos que se quiera forzar una actualización explícita.
        }

        public async Task DeleteProductoAsync(int id)
        {
            var producto = await GetEntityOrThrowAsync(_productoRepository, id, "Producto");

            await _productoRepository.DeleteAsync(producto);
        }

        protected override ProductoDto MapToDto(Producto producto)
        {
            return MapToDto(producto, null);
        }

        private ProductoDto MapToDto(Producto producto, Precio? precio)
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
                    Precio_Unitario = l.Precio_Unitario
                })
                .ToList();

            // Mapear datos del primer lote a las propiedades planas del DTO para que aparezcan en el formulario
            var primerLote = lotesOrdenados.FirstOrDefault();

            return new ProductoDto
            {
                Id_Pro = producto.Id_Pro,
                Tip_Pro = producto.Tip_Pro,
                Nom_Pro = producto.Nom_Pro,
                Marca = producto.Marca,
                Imagen = producto.Imagen,
                StockTotal = producto.Lotes.Sum(l => l.Cantidad_Disponible),
                NumLotes = producto.Lotes.Count,
                Lotes = lotesOrdenados,
                Precio_Venta = precio?.Precio_Venta ?? 0,
                
                // Mapeo de campos planos desde el primer lote (si existe)
                Fec_Ent = primerLote?.Fec_Ent,
                Fec_Exp = primerLote?.Fec_Exp,
                Cantidad_Recibida = primerLote?.Cantidad_Recibida,
                Cantidad_Disponible = primerLote?.Cantidad_Disponible,
                Precio_Unitario = primerLote?.Precio_Unitario
            };
        }

        protected override Producto MapToEntity(ProductoDto dto)
        {
            return new Producto
            {
                Tip_Pro = dto.Tip_Pro,
                Nom_Pro = dto.Nom_Pro,
                Marca = dto.Marca,
                Imagen = dto.Imagen
            };
        }

        private Producto MapToEntity(ProductoConLoteDto dto)
        {
            return new Producto
            {
                Tip_Pro = dto.Tip_Pro,
                Nom_Pro = dto.Nom_Pro,
                Marca = dto.Marca,
                Imagen = dto.Imagen
            };
        }

        public async Task<int> ConsolidarProductosDuplicadosAsync()
        {
            // 1. Obtener todos los productos con lotes y precios
            var productos = await _productoRepository.GetProductosForConsolidationAsync();

            // 2. Agrupar por nombre (case-insensitive)
            var gruposDuplicados = productos
                .GroupBy(p => p.Nom_Pro.ToLower())
                .Where(g => g.Count() > 1)
                .ToList();

            int consolidadas = 0;

            foreach (var grupo in gruposDuplicados)
            {
                var productosDelGrupo = grupo.OrderBy(p => p.Id_Pro).ToList();
                var productoBase = productosDelGrupo.First(); // El más antiguo (menor ID)
                var productosAEliminar = productosDelGrupo.Skip(1).ToList();

                // Consolidar
                foreach (var productoDuplicado in productosAEliminar)
                {
                    // Mover lotes al producto base
                    foreach (var lote in productoDuplicado.Lotes.ToList())
                    {
                        lote.Id_Pro_Per = productoBase.Id_Pro;
                        await _loteRepository.UpdateAsync(lote);
                    }

                    // Mover precios al producto base
                    foreach (var precio in productoDuplicado.Precios.ToList())
                    {
                        precio.Id_Pro_Per = productoBase.Id_Pro;
                        await _precioRepository.UpdateAsync(precio);
                    }

                    // Eliminar el producto duplicado
                    await _productoRepository.DeleteAsync(productoDuplicado);
                }

                consolidadas++;
            }

            return consolidadas;
        }
    }
}
