using API.Services;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrecioController : ControllerBase
    {
        private readonly PrecioService _service;

        public PrecioController(PrecioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Precio precio)
        {
            var result = await _service.CreateAsync(precio);
            return Ok(result);
        }
    }
}
