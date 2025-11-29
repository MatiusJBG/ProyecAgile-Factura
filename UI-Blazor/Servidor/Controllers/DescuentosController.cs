using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace UI_Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DescuentosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DescuentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Descuentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DescuentoProducto>>> GetAllDescuentos()
        {
            return await _context.DescuentosProductos
                .Where(d => d.Activo)
                .OrderByDescending(d => d.FechaInicio)
                .ToListAsync();
        }

        // GET: api/Descuentos/producto/5
        [HttpGet("producto/{idProducto}")]
        public async Task<ActionResult<IEnumerable<DescuentoProducto>>> GetDescuentosPorProducto(int idProducto)
        {
            return await _context.DescuentosProductos
                .Where(d => d.Id_Pro_Per == idProducto)
                .OrderByDescending(d => d.FechaInicio)
                .ToListAsync();
        }

        // GET: api/Descuentos/activos/producto/5
        [HttpGet("activos/producto/{idProducto}")]
        public async Task<ActionResult<DescuentoProducto?>> GetDescuentoActivo(int idProducto)
        {
            var today = DateTime.Today;
            return await _context.DescuentosProductos
                .Where(d => d.Id_Pro_Per == idProducto && 
                            d.Activo && 
                            d.FechaInicio <= today && 
                            (d.FechaFin == null || d.FechaFin >= today))
                .OrderByDescending(d => d.FechaInicio)
                .FirstOrDefaultAsync();
        }

        // POST: api/Descuentos
        [HttpPost]
        public async Task<ActionResult<DescuentoProducto>> PostDescuento(DescuentoProducto descuento)
        {
            _context.DescuentosProductos.Add(descuento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDescuentoActivo", new { idProducto = descuento.Id_Pro_Per }, descuento);
        }

        // PUT: api/Descuentos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDescuento(int id, DescuentoProducto descuento)
        {
            if (id != descuento.Id_Desc)
            {
                return BadRequest();
            }

            _context.Entry(descuento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DescuentoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Descuentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDescuento(int id)
        {
            var descuento = await _context.DescuentosProductos.FindAsync(id);
            if (descuento == null)
            {
                return NotFound();
            }

            _context.DescuentosProductos.Remove(descuento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DescuentoExists(int id)
        {
            return _context.DescuentosProductos.Any(e => e.Id_Desc == id);
        }
    }
}
