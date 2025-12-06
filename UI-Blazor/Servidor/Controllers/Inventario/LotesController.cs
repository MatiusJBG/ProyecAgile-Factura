using Application.DTOs.Producto;
using Application.Services.Inventario;
using Microsoft.AspNetCore.Mvc;

namespace UI_Blazor.Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotesController : ControllerBase
    {
        private readonly LoteService _loteService;
        private readonly ILogger<LotesController> _logger;

        public LotesController(LoteService loteService, ILogger<LotesController> logger)
        {
            _loteService = loteService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoteDto>>> GetLotes()
        {
            try
            {
                var lotes = await _loteService.GetAllLotesAsync();
                return Ok(lotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lotes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoteDto>> GetLote(int id)
        {
            try
            {
                var lote = await _loteService.GetLoteByIdAsync(id);
                if (lote == null)
                {
                    return NotFound($"Lote con ID {id} no encontrado");
                }
                return Ok(lote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lote {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("producto/{idProducto}")]
        public async Task<ActionResult<IEnumerable<LoteDto>>> GetLotesByProducto(int idProducto)
        {
            try
            {
                var lotes = await _loteService.GetLotesByProductoAsync(idProducto);
                return Ok(lotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lotes del producto {IdProducto}", idProducto);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<LoteDto>>> GetLotesDisponibles()
        {
            try
            {
                var lotes = await _loteService.GetLotesDisponiblesAsync();
                return Ok(lotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lotes disponibles");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<LoteDto>> CreateLote([FromBody] LoteDto loteDto)
        {
            try
            {
                var creado = await _loteService.CreateLoteAsync(loteDto);
                return CreatedAtAction(nameof(GetLote), new { id = creado.Id_Lote }, creado);
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
                _logger.LogError(ex, "Error al crear lote");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLote(int id, [FromBody] LoteDto loteDto)
        {
            try
            {
                await _loteService.UpdateLoteAsync(id, loteDto);
                return NoContent();
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
                _logger.LogError(ex, "Error al actualizar lote {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLote(int id)
        {
            try
            {
                await _loteService.DeleteLoteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lote {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
