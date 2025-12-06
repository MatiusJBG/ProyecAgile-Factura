using Application.Services.Inventario;
using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Microsoft.AspNetCore.Mvc;

namespace UI_Blazor.Servidor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DescuentosController : ControllerBase
    {
        private readonly DescuentoService _descuentoService;

        public DescuentosController(DescuentoService descuentoService)
        {
            _descuentoService = descuentoService;
        }

        // GET: api/Descuentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DescuentoProducto>>> GetAllDescuentos()
        {
            var descuentos = await _descuentoService.GetAllActiveDescuentosAsync();
            return Ok(descuentos);
        }

        // GET: api/Descuentos/producto/5
        [HttpGet("producto/{idProducto}")]
        public async Task<ActionResult<IEnumerable<DescuentoProducto>>> GetDescuentosPorProducto(int idProducto)
        {
            var descuentos = await _descuentoService.GetDescuentosByProductoAsync(idProducto);
            return Ok(descuentos);
        }

        // GET: api/Descuentos/activos/producto/5
        [HttpGet("activos/producto/{idProducto}")]
        public async Task<ActionResult<DescuentoProducto?>> GetDescuentoActivo(int idProducto)
        {
            var descuento = await _descuentoService.GetActiveDescuentoByProductoAsync(idProducto);
            return Ok(descuento); // Si es null, retorna null (204 No Content para HttpClient.GetFromJsonAsync?) o 200 con body null
        }

        // POST: api/Descuentos
        [HttpPost]
        public async Task<ActionResult<DescuentoProducto>> PostDescuento(DescuentoProducto descuento)
        {
            var creado = await _descuentoService.CreateDescuentoAsync(descuento);
            return CreatedAtAction("GetDescuentoActivo", new { idProducto = creado.Id_Pro_Per }, creado);
        }

        // PUT: api/Descuentos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDescuento(int id, DescuentoProducto descuento)
        {
            if (id != descuento.Id_Desc)
            {
                return BadRequest();
            }

            try
            {
                await _descuentoService.UpdateDescuentoAsync(descuento);
            }
            catch (KeyNotFoundException)
            {
                 return NotFound();
            }
            catch (Exception)
            {
                 throw;
            }

            return NoContent();
        }

        // DELETE: api/Descuentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDescuento(int id)
        {
            await _descuentoService.DeleteDescuentoAsync(id);
            return NoContent();
        }
    }
}
