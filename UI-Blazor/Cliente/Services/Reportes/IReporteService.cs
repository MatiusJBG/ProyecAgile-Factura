using Application.DTOs.Reportes;
using System;
using System.Threading.Tasks;

namespace Cliente.Services.Reportes
{
    public interface IReporteService
    {
        Task<ReporteFinancieroDto> GetReporteFinancieroAsync(DateTime? fechaInicio, DateTime? fechaFin);
        Task<ReporteVentasInventarioDto> GetReporteVentasInventarioAsync(DateTime? fechaInicio, DateTime? fechaFin);
    }
}
