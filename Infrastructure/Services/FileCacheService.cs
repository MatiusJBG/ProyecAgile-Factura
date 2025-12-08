using System.Text.Json;
using Application.DTOs.Cliente;
using Application.DTOs.Producto;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class FileCacheService : IFileCacheService
    {
        private readonly string _clienteCachePath;
        private readonly string _productoCachePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileCacheService()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var cacheDir = Path.Combine(basePath, "CacheData");
            
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            _clienteCachePath = Path.Combine(cacheDir, "cache_clientes.json");
            _productoCachePath = Path.Combine(cacheDir, "cache_productos.json");
            
            _jsonOptions = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task SaveClientesCacheAsync(IEnumerable<ClienteDto> clientes)
        {
            try
            {
                using var stream = File.Create(_clienteCachePath);
                await JsonSerializer.SerializeAsync(stream, clientes, _jsonOptions);
            }
            catch (Exception ex)
            {
                // In a real scenario, log this
                Console.WriteLine($"Error writing client cache: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ClienteDto>> GetClientesCacheAsync()
        {
            if (!File.Exists(_clienteCachePath)) 
                return Enumerable.Empty<ClienteDto>();

            try
            {
                using var stream = File.OpenRead(_clienteCachePath);
                return await JsonSerializer.DeserializeAsync<IEnumerable<ClienteDto>>(stream, _jsonOptions) 
                       ?? Enumerable.Empty<ClienteDto>();
            }
            catch
            {
                return Enumerable.Empty<ClienteDto>();
            }
        }

        public Task InvalidateClientesCacheAsync()
        {
            try
            {
                if (File.Exists(_clienteCachePath))
                {
                    File.Delete(_clienteCachePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error invalidating client cache: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        public async Task SaveProductosCacheAsync(IEnumerable<ProductoDto> productos)
        {
            try
            {
                using var stream = File.Create(_productoCachePath);
                await JsonSerializer.SerializeAsync(stream, productos, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing product cache: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ProductoDto>> GetProductosCacheAsync()
        {
            if (!File.Exists(_productoCachePath)) 
                return Enumerable.Empty<ProductoDto>();

            try
            {
                using var stream = File.OpenRead(_productoCachePath);
                return await JsonSerializer.DeserializeAsync<IEnumerable<ProductoDto>>(stream, _jsonOptions) 
                       ?? Enumerable.Empty<ProductoDto>();
            }
            catch
            {
                return Enumerable.Empty<ProductoDto>();
            }
        }

        public Task InvalidateProductosCacheAsync()
        {
            try
            {
                if (File.Exists(_productoCachePath))
               {
                    File.Delete(_productoCachePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error invalidating product cache: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        public Task<bool> IsCacheValidAsync()
        {
            // Simple check if files exist and have content
            var valid = File.Exists(_clienteCachePath) && new FileInfo(_clienteCachePath).Length > 0 &&
                        File.Exists(_productoCachePath) && new FileInfo(_productoCachePath).Length > 0;
            return Task.FromResult(valid);
        }
    }
}
