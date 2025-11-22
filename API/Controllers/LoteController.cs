using API.Services;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoteController : ControllerBase
    {
        private readonly LoteService _service;

        public LoteController(LoteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lote = await _service.GetByIdAsync(id);
            return lote == null ? NotFound() : Ok(lote);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Lote lote)
        {
            var created = await _service.CreateAsync(lote);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Lote lote)
        {
            if (id != lote.Id_Lote) return BadRequest();

            var updated = await _service.UpdateAsync(lote);
            return updated ? Ok(lote) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
