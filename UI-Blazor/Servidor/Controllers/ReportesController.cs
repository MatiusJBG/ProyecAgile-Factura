using Application.DTOs.Reportes;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace UI_Blazor.Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReporteService reporteService, ILogger<ReportesController> logger)
        {
            _reporteService = reporteService;
            _logger = logger;
        }

        [HttpGet("financiero")]
        public async Task<ActionResult<ReporteFinancieroDto>> GetReporteFinanciero(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var reporte = await _reporteService.GetReporteFinancieroAsync(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte financiero");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("ventas-inventario")]
        public async Task<ActionResult<ReporteVentasInventarioDto>> GetReporteVentasInventario(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var reporte = await _reporteService.GetReporteVentasInventarioAsync(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de ventas e inventario");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
