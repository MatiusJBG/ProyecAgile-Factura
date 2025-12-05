using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsolidarProductosDuplicados
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-QCUD3LD\\SQLEXPRESS;Database=FacturacionDB;Trusted_Connection=True;TrustServerCertificate=True;");

            using var context = new ApplicationDbContext(optionsBuilder.Options);

            Console.WriteLine("🔍 Buscando productos duplicados...\n");

            // Obtener todos los productos con sus lotes
            var productos = await context.Productos
                .Include(p => p.Lotes)
                .Include(p => p.Precios)
                .ToListAsync();

            // Agrupar por nombre (case-insensitive)
            var gruposDuplicados = productos
                .GroupBy(p => p.Nom_Pro.ToLower())
                .Where(g => g.Count() > 1)
                .ToList();

            if (!gruposDuplicados.Any())
            {
                Console.WriteLine("✅ No se encontraron productos duplicados.");
                return;
            }

            Console.WriteLine($"📦 Se encontraron {gruposDuplicados.Count} productos duplicados:\n");

            foreach (var grupo in gruposDuplicados)
            {
                var productosDelGrupo = grupo.OrderBy(p => p.Id_Pro).ToList();
                var productoBase = productosDelGrupo.First(); // El más antiguo (menor ID)
                var productosAEliminar = productosDelGrupo.Skip(1).ToList();

                Console.WriteLine($"  📌 Producto: {productoBase.Nom_Pro}");
                Console.WriteLine($"     - Producto base (mantener): ID {productoBase.Id_Pro}");
                Console.WriteLine($"     - Duplicados a consolidar: {string.Join(", ", productosAEliminar.Select(p => $"ID {p.Id_Pro}"))}");

                // Consolidar lotes
                foreach (var productoDuplicado in productosAEliminar)
                {
                    // Mover lotes al producto base
                    foreach (var lote in productoDuplicado.Lotes)
                    {
                        lote.Id_Pro_Per = productoBase.Id_Pro;
                        Console.WriteLine($"       ✓ Moviendo lote {lote.Id_Lote} (Fecha: {lote.Fec_Ent:dd/MM/yyyy}, Cant: {lote.Cantidad_Disponible})");
                    }

                    // Mover precios al producto base
                    foreach (var precio in productoDuplicado.Precios)
                    {
                        precio.Id_Pro_Per = productoBase.Id_Pro;
                    }

                    // Eliminar el producto duplicado
                    context.Productos.Remove(productoDuplicado);
                }

                Console.WriteLine();
            }

            // Guardar cambios
            Console.WriteLine("💾 Guardando cambios en la base de datos...");
            await context.SaveChangesAsync();

            Console.WriteLine("\n✅ ¡Consolidación completada exitosamente!");
            Console.WriteLine("\n📊 Resumen:");
            
            foreach (var grupo in gruposDuplicados)
            {
                var productoBase = grupo.OrderBy(p => p.Id_Pro).First();
                var lotesConsolidados = await context.Lotes
                    .Where(l => l.Id_Pro_Per == productoBase.Id_Pro)
                    .OrderBy(l => l.Fec_Ent)
                    .ToListAsync();

                Console.WriteLine($"\n  {productoBase.Nom_Pro} (ID {productoBase.Id_Pro}):");
                Console.WriteLine($"    Total de lotes: {lotesConsolidados.Count}");
                Console.WriteLine($"    Stock total: {lotesConsolidados.Sum(l => l.Cantidad_Disponible)}");
                Console.WriteLine($"    Lotes (ordenados por FIFO):");
                
                foreach (var lote in lotesConsolidados)
                {
                    Console.WriteLine($"      - Lote {lote.Id_Lote}: {lote.Fec_Ent:dd/MM/yyyy} | Cant: {lote.Cantidad_Disponible} | Precio: ${lote.Precio_Unitario:F2}");
                }
            }
        }
    }
}
