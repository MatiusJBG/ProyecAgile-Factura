using Application.DTOs.Cliente;
using Application.Services.Clientes;
using Microsoft.AspNetCore.Mvc;

namespace UI_Blazor.Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(ClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            try
            {
                var clientes = await _clienteService.GetAllClientesAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            try
            {
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                if (cliente == null)
                {
                    return NotFound($"Cliente con ID {id} no encontrado");
                }
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> SearchClientes([FromQuery] string q = "")
        {
            try
            {
                var clientes = await _clienteService.SearchClientesAsync(q);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar clientes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CreateCliente([FromBody] ClienteDto clienteDto)
        {
            try
            {
                var creado = await _clienteService.CreateClienteAsync(clienteDto);
                return CreatedAtAction(nameof(GetCliente), new { id = creado.Id_Cli }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteDto clienteDto)
        {
            try
            {
                await _clienteService.UpdateClienteAsync(id, clienteDto);
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
                _logger.LogError(ex, "Error al actualizar cliente {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                await _clienteService.DeleteClienteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
