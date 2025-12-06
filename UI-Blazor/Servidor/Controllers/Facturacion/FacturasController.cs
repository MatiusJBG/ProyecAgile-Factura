using Application.DTOs.Factura;
using Application.Services.Facturacion;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Common;

namespace UI_Blazor.Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly FacturaService _facturaService;
        private readonly ILogger<FacturasController> _logger;

        public FacturasController(FacturaService facturaService, ILogger<FacturasController> logger)
        {
            _facturaService = facturaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<FacturaDto>>> GetFacturas(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = "",
            [FromQuery] string estado = "")
        {
            try
            {
                var result = await _facturaService.GetFacturasPagedAsync(page, pageSize, searchTerm, estado);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener facturas paginadas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaDto>> GetFactura(int id)
        {
            try
            {
                var factura = await _facturaService.GetFacturaByIdAsync(id);
                if (factura == null)
                {
                    return NotFound($"Factura con ID {id} no encontrada");
                }
                return Ok(factura);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener factura {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FacturaDto>> CreateFactura([FromBody] FacturaDto facturaDto)
        {
            _logger.LogInformation("Recibida solicitud POST para crear factura. Cliente: {ClienteId}, Total: {Total}", 
                facturaDto?.Id_Cli_Per, facturaDto?.Tot_Fac_Con_IVA);

            try
            {
                if (facturaDto == null)
                {
                    _logger.LogWarning("El cuerpo de la solicitud es nulo");
                    return BadRequest("El cuerpo de la solicitud no puede ser nulo");
                }

                var creada = await _facturaService.CreateFacturaAsync(facturaDto);
                return CreatedAtAction(nameof(GetFactura), new { id = creada.Id_Fac }, creada);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear factura");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("test")]
        public IActionResult TestPost()
        {
            return Ok("POST funciona correctamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFactura(int id, [FromBody] FacturaDto facturaDto)
        {
            try
            {
                await _facturaService.UpdateFacturaAsync(id, facturaDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar factura {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            try
            {
                await _facturaService.DeleteFacturaAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar factura {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
