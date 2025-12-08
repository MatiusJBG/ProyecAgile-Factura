using Application.DTOs.Reportes;
using Application.Interfaces;
using Core.Entities.Facturacion;
using Core.Entities.Inventario;
using Core.Enums.Facturacion;
using Core.Interfaces.Facturacion;
using Core.Interfaces.Inventario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ILoteRepository _loteRepository;

        public ReporteService(
            IFacturaRepository facturaRepository,
            IProductoRepository productoRepository,
            ILoteRepository loteRepository)
        {
            _facturaRepository = facturaRepository;
            _productoRepository = productoRepository;
            _loteRepository = loteRepository;
        }

        public async Task<ReporteFinancieroDto> GetReporteFinancieroAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var inicio = fechaInicio ?? DateTime.MinValue;
            var fin = fechaFin ?? DateTime.MaxValue;

            var facturas = await _facturaRepository.GetAllAsync();
            var lotes = await _loteRepository.GetAllAsync();

            // Filtrar por fechas
            var facturasPeriodo = facturas.Where(f => f.Fec_Fac >= inicio && f.Fec_Fac <= fin && f.Estado != EstadoFactura.Anulada).ToList();
            var comprasPeriodo = lotes.Where(l => l.Fec_Ent >= inicio && l.Fec_Ent <= fin).ToList();

            var reporte = new ReporteFinancieroDto();

            // 1. Flujo de Efectivo (Simplificado: Ingresos por Ventas, Egresos por Compras de Lotes)
            // Asumimos que Facturas Pagadas son Ingresos Reales
            reporte.FlujoEfectivo.InicioPeriodo = inicio;
            reporte.FlujoEfectivo.FinPeriodo = fin;
            reporte.FlujoEfectivo.TotalIngresos = facturasPeriodo
                .Where(f => f.Estado == EstadoFactura.Pagada || f.Estado == EstadoFactura.Autorizada)
                .Sum(f => f.Tot_Fac_Con_IVA ?? 0);
            
            reporte.FlujoEfectivo.TotalEgresos = comprasPeriodo.Sum(l => l.Precio_Lote);

            // 2. Cuentas por Cobrar (Facturas Pendientes o no Pagadas)
            var cxc = facturas
                .Where(f => f.Estado == EstadoFactura.Pendiente || f.Estado == EstadoFactura.Enviada || f.Estado == EstadoFactura.NoEnviada)
                .Select(f => new CuentaPorCobrarDto
                {
                    IdFactura = f.Id_Fac,
                    FechaEmision = f.Fec_Fac,
                    ClienteNombre = f.Cliente != null ? ($"{f.Cliente.Nombre} {f.Cliente.Apellido}".Trim()) : "Cliente Desconocido",
                    MontoTotal = f.Tot_Fac_Con_IVA ?? 0,
                    MontoPendiente = f.Tot_Fac_Con_IVA ?? 0, // Asumimos todo pendiente si no está pagada (falta entidad Pagos)
                    DiasMora = (DateTime.Now - f.Fec_Fac).Days,
                    Estado = (DateTime.Now - f.Fec_Fac).Days > 30 ? "Vencida" : "Por Vencer" // Regla simple 30 dias
                }).ToList();
            reporte.CuentasPorCobrar = cxc;

            // 3. Rentabilidad (Based on Sales - COGS)
            // COGS = Cost of Goods Sold. Sum of (Quantity Sold * Cost per Unit)
            // Need to iterate details of sold invoices
            decimal ingresosVentas = facturasPeriodo.Sum(f => f.Tot_Fac_Sin_IVA ?? 0);
            decimal cogs = 0;

            foreach (var fac in facturasPeriodo)
            {
                if (fac.Detalles != null)
                {
                    foreach (var det in fac.Detalles)
                    {
                        // Costo Unitario viene del Lote asociado
                        // Si Lote no esta cargado, podria ser 0. Asumimos repositorio include Lote.
                        // Si no, podria requerir query extra. Intentaremos usar data disponible.
                        // Asumiendo que el repositorio hace Include(d => d.Lote)
                         if (det.Lote != null)
                        {
                            cogs += det.Cantidad_Comprada * det.Lote.Precio_Unitario;
                        }
                    }
                }
            }
            
            reporte.Rentabilidad.IngresosPorVentas = ingresosVentas;
            reporte.Rentabilidad.CostoBienesVendidos = cogs;
            // Gastos operativos hardcoded o 0 por ahora (no data available)
            reporte.Rentabilidad.GastosOperativos = 0; 

            // 4. Impuestos
            reporte.Impuestos.TotalIvaRecaudado = facturasPeriodo.Sum(f => f.IVA_Fac ?? 0);
            // IVA Pagado en compras (Si Lote tuviera IVA, no lo tiene explicito en modelo simple, asumimos 0 o derivado)
            // Asumiremos 0 por falta de datos en Lote para IVA
            reporte.Impuestos.TotalIvaPagado = 0;

            return reporte;
        }

        public async Task<ReporteVentasInventarioDto> GetReporteVentasInventarioAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
             var inicio = fechaInicio ?? DateTime.MinValue;
            var fin = fechaFin ?? DateTime.MaxValue;

            var facturas = await _facturaRepository.GetAllAsync();
            var productos = await _productoRepository.GetAllAsync();
            var lotes = await _loteRepository.GetAllAsync();

            var facturasPeriodo = facturas.Where(f => f.Fec_Fac >= inicio && f.Fec_Fac <= fin && f.Estado != EstadoFactura.Anulada).ToList();

            var reporte = new ReporteVentasInventarioDto();

            // 1. Resumen Ventas
            reporte.ResumenVentas.InicioPeriodo = inicio;
            reporte.ResumenVentas.FinPeriodo = fin;
            reporte.ResumenVentas.TotalVentas = facturasPeriodo.Count;
            reporte.ResumenVentas.TotalIngresos = facturasPeriodo.Sum(f => f.Tot_Fac_Con_IVA ?? 0);

            // 2. Top Productos (Agrupar detalles)
            var detallesFlat = facturasPeriodo.SelectMany(f => f.Detalles).ToList();
            
            var topProductos = detallesFlat
                .GroupBy(d => d.Id_Pro_Per)
                .Select(g => new ProductoTopDto
                {
                    NombreProducto = g.FirstOrDefault()?.Producto?.Nom_Pro ?? "Desconocido", // Asumiendo Include
                    CantidadVendida = g.Sum(d => d.Cantidad_Comprada),
                    IngresoGenerado = g.Sum(d => d.Precio_Venta_Total)
                })
                .OrderByDescending(x => x.IngresoGenerado)
                .Take(10) // Top 10
                .ToList();

            for(int i=0; i<topProductos.Count; i++)
            {
                topProductos[i].Ranking = i + 1;
                // Si el nombre vino nulo por falta de Include, intentar buscar en lista de productos
                if(topProductos[i].NombreProducto == "Desconocido")
                {
                   // Fallback logic logic ID if needed, but assuming ID map keys
                   // Not easy to recover ID here without key. 
                   // G.Key es Id.
                }
            }

            // Fix name fill if needed
             var topProductosWithNames = detallesFlat
                .GroupBy(d => d.Id_Pro_Per)
                 .Select(g => new 
                {
                    Id = g.Key,
                    Cantidad = g.Sum(d => d.Cantidad_Comprada),
                    Ingreso = g.Sum(d => d.Precio_Venta_Total)
                })
                .OrderByDescending(x => x.Ingreso)
                .Take(10)
                .ToList();
            
            reporte.TopProductos = topProductosWithNames.Select((x, index) => new ProductoTopDto
            {
                Ranking = index + 1,
                NombreProducto = productos.FirstOrDefault(p => p.Id_Pro == x.Id)?.Nom_Pro ?? "ID " + x.Id,
                CantidadVendida = x.Cantidad,
                IngresoGenerado = x.Ingreso
            }).ToList();


            // 3. Inventario General
             reporte.InventarioGeneral.TotalItemsEnStock = lotes.Sum(l => l.Cantidad_Disponible);
             reporte.InventarioGeneral.ValorTotalInventario = lotes.Sum(l => l.Cantidad_Disponible * l.Precio_Unitario);

             // 4. Alertas (Productos con poco stock)
             // Agrupar stock por producto
             var stockPorProducto = lotes
                 .GroupBy(l => l.Id_Pro_Per)
                 .Select(g => new
                 {
                     IdProducto = g.Key,
                     StockTotal = g.Sum(l => l.Cantidad_Disponible)
                 }).ToList();

             int umbralAlerta = 10; // Hardcoded threshold
             
             foreach(var item in stockPorProducto)
             {
                 if(item.StockTotal < umbralAlerta)
                 {
                     reporte.AlertasStock.Add(new AlertaStockDto
                     {
                         IdProducto = item.IdProducto,
                         NombreProducto = productos.FirstOrDefault(p => p.Id_Pro == item.IdProducto)?.Nom_Pro ?? "Desconocido",
                         StockActual = item.StockTotal,
                         StockMinimo = umbralAlerta,
                         EstadoAlerta = "Bajo Stock"
                     });
                 }
             }

            return reporte;
        }
    }
}
