using Core.Interfaces.Clientes;
using Application.Interfaces;
using Core.Interfaces.Inventario;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services
{
    public class FileCacheWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(5); // Update cache every 5 minutes

        public FileCacheWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateCacheAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cache Worker Error: {ex.Message}");
                }

                await Task.Delay(_updateInterval, stoppingToken);
            }
        }

        private async Task UpdateCacheAsync(CancellationToken token)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var fileCache = scope.ServiceProvider.GetRequiredService<IFileCacheService>();
                var clienteRepo = scope.ServiceProvider.GetRequiredService<IClienteRepository>();
                var productoRepo = scope.ServiceProvider.GetRequiredService<IProductoRepository>();

                // 1. Fetch from DB (Repositories)
                // Note: We need a method to get DTOs directly or map Entities -> DTOs here.
                // Assuming repositories return Entities, we might need a Service here instead, 
                // BUT Services inject Repositories, so circularly we should be careful.
                // Better: Use Repositories to get Entities, then map to DTOs manually or use AutoMapper if available.
                // However, the prompt asked for "hilos en archivos... intermediario entre la consulta y la base".
                // Let's assume we can fetch all and map simple properties.
                
                var clientes = await clienteRepo.GetAllAsync();
                
                // Manual mapping to avoid circular dependencies with App Layer Services if strictly separated
                // But DTOs are in Application, so we can use them.
                var clienteDtos = clientes.Select(c => new Application.DTOs.Cliente.ClienteDto
                {
                    Id_Cli = c.Id_Cli,
                    Tipo_Cliente = c.Tipo_Cliente,
                    Tipo_Documento = c.Tipo_Documento,
                    Num_Documento = c.Num_Documento,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido ?? string.Empty,
                    Direccion = c.Direccion ?? string.Empty,
                    Telefono = c.Telefono ?? string.Empty,
                    Correo = c.Correo ?? string.Empty,
                    Activo = c.Activo
                }).ToList();

                await fileCache.SaveClientesCacheAsync(clienteDtos);

                var productos = await productoRepo.GetProductosWithStockAsync();
                var productoDtos = productos.Select(p => new Application.DTOs.Producto.ProductoDto
                {
                    Id_Pro = p.Id_Pro,
                    Nom_Pro = p.Nom_Pro,
                    Tip_Pro = p.Tip_Pro,
                    Marca = p.Marca,
                    Imagen = p.Imagen,
                    Precio_Venta = p.Precios.OrderByDescending(pr => pr.Fecha_Actualizacion).FirstOrDefault()?.Precio_Venta ?? 0,
                    // Note: Lotes mapping might be complex, keeping it simple for search cache
                    Lotes = p.Lotes.Select(l => new Application.DTOs.Producto.LoteDto 
                    { 
                        Id_Lote = l.Id_Lote,
                        Cantidad_Disponible = l.Cantidad_Disponible,
                        Fec_Ent = l.Fec_Ent,
                        Fec_Exp = l.Fec_Exp
                    }).ToList(),
                    StockTotal = p.Lotes.Sum(l => l.Cantidad_Disponible)
                }).ToList();

                await fileCache.SaveProductosCacheAsync(productoDtos);
                
                Console.WriteLine($"[Cache Worker] Cache updated at {DateTime.Now}");
            }
        }
    }
}
