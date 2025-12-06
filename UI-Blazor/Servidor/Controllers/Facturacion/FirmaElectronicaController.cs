using Core.Entities.Clientes; using Core.Entities.Facturacion; using Core.Entities.Inventario; using Core.Entities.Auth; using Core.Entities.Certificados; using Core.Entities.Common;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Services.Facturacion; using Infrastructure.Services.Certificados;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace UI_Blazor.Servidor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmaElectronicaController : ControllerBase
    {
        private readonly IFirmaElectronicaService _firmaService;

        public FirmaElectronicaController(IFirmaElectronicaService firmaService)
        {
            _firmaService = firmaService;
        }

        // POST: api/FirmaElectronica/firmar/5
        [HttpPost("firmar/{idFactura}")]
        public async Task<ActionResult<FirmaElectronica>> FirmarFactura(int idFactura)
        {
            try
            {
                var firma = await _firmaService.FirmarFacturaAsync(idFactura);
                return Ok(firma);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al firmar factura: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/FirmaElectronica/validar/5
        [HttpGet("validar/{idFactura}")]
        public async Task<ActionResult<bool>> ValidarFirma(int idFactura)
        {
            try
            {
                var esValida = await _firmaService.ValidarFirmaAsync(idFactura);
                return Ok(new { valida = esValida });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/FirmaElectronica/5
        [HttpGet("{idFactura}")]
        public async Task<ActionResult<FirmaElectronica>> GetFirma(int idFactura)
        {
            var firma = await _firmaService.GetFirmaPorFacturaAsync(idFactura);
            
            if (firma == null)
            {
                return NotFound(new { error = "No se encontró firma para esta factura" });
            }

            return Ok(firma);
        }

        // GET: api/FirmaElectronica/xml/5
        [HttpGet("xml/{idFactura}")]
        public async Task<ActionResult<string>> GetXmlFactura(int idFactura)
        {
            try
            {
                // Aquí necesitarías obtener la factura del contexto
                // Por ahora retornamos un placeholder
                return Ok(new { xml = "XML de la factura" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
