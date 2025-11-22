using API.Services;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly FacturaService _service;

        public FacturaController(FacturaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var factura = await _service.GetByIdAsync(id);
            return factura == null ? NotFound() : Ok(factura);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Factura factura)
        {
            var created = await _service.CreateAsync(factura);

            return created == null
                ? BadRequest("Error al crear factura")
                : Ok(created);
        }
    }
}
