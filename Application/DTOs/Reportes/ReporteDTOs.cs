using System;
using System.Collections.Generic;

namespace Application.DTOs.Reportes
{
    // --- Reporte Financiero ---
    public class ReporteFinancieroDto
    {
        public FlujoEfectivoDto FlujoEfectivo { get; set; } = new();
        public List<CuentaPorCobrarDto> CuentasPorCobrar { get; set; } = new();
        public List<CuentaPorPagarDto> CuentasPorPagar { get; set; } = new();
        public RentabilidadDto Rentabilidad { get; set; } = new();
        public ImpuestosDto Impuestos { get; set; } = new();
    }

    public class FlujoEfectivoDto
    {
        public DateTime InicioPeriodo { get; set; }
        public DateTime FinPeriodo { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalEgresos { get; set; }
        public decimal SaldoNeto => TotalIngresos - TotalEgresos;
    }

    public class CuentaPorCobrarDto
    {
        public int IdFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public decimal MontoPendiente { get; set; }
        public int DiasMora { get; set; }
        public string Estado { get; set; } = string.Empty; // Vencida, Por Vencer
    }

    public class CuentaPorPagarDto
    {
        public string IdFacturaProveedor { get; set; } = string.Empty;
        public string ProveedorNombre { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoAdeudado { get; set; }
    }

    public class RentabilidadDto
    {
        public decimal IngresosPorVentas { get; set; }
        public decimal CostoBienesVendidos { get; set; } // COGS
        public decimal UtilidadBruta => IngresosPorVentas - CostoBienesVendidos;
        public decimal GastosOperativos { get; set; }
        public decimal UtilidadNeta => UtilidadBruta - GastosOperativos;
    }

    public class ImpuestosDto
    {
        public decimal TotalIvaRecaudado { get; set; }
        public decimal TotalIvaPagado { get; set; }
        public decimal TotalOtrosImpuestos { get; set; }
    }

    // --- Reporte Ventas e Inventario ---
    public class ReporteVentasInventarioDto
    {
        public ResumenVentasDto ResumenVentas { get; set; } = new();
        public List<ProductoTopDto> TopProductos { get; set; } = new();
        public InventarioGeneralDto InventarioGeneral { get; set; } = new();
        public List<AlertaStockDto> AlertasStock { get; set; } = new();
    }

    public class ResumenVentasDto
    {
        public DateTime InicioPeriodo { get; set; }
        public DateTime FinPeriodo { get; set; }
        public int TotalVentas { get; set; }
        public decimal TotalIngresos { get; set; }
        // Se pueden agregar desgloses por cliente si se requiere
    }

    public class ProductoTopDto
    {
        public int Ranking { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int CantidadVendida { get; set; }
        public decimal IngresoGenerado { get; set; }
    }

    public class InventarioGeneralDto
    {
        public int TotalItemsEnStock { get; set; }
        public decimal ValorTotalInventario { get; set; }
    }

    public class AlertaStockDto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public string EstadoAlerta { get; set; } = string.Empty; // Requerido, Critico
    }
}
