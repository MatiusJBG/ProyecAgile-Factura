using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services.Facturacion;
using Infrastructure.Services.Sri;
using Core.Interfaces.Facturacion;
using Core.Interfaces.Certificados;
using Core.Entities.Facturacion;
using System.Text.Json;

namespace UI_Blazor.Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SriController : ControllerBase
    {
        private readonly FacturaXmlService _facturaXmlService;
        private readonly IFirmaElectronicaService _firmaElectronicaService;
        private readonly SriRecepcionClient _sriRecepcionClient;
        private readonly SriAutorizacionClient _sriAutorizacionClient;
        private readonly IFacturaRepository _facturaRepository;
        private readonly ICertificadoService _certificadoService;

        public SriController(
            FacturaXmlService facturaXmlService,
            IFirmaElectronicaService firmaElectronicaService,
            SriRecepcionClient sriRecepcionClient,
            SriAutorizacionClient sriAutorizacionClient,
            IFacturaRepository facturaRepository,
            ICertificadoService certificadoService)
        {
            _facturaXmlService = facturaXmlService;
            _firmaElectronicaService = firmaElectronicaService;
            _sriRecepcionClient = sriRecepcionClient;
            _sriAutorizacionClient = sriAutorizacionClient;
            _facturaRepository = facturaRepository;
            _certificadoService = certificadoService;
        }

        [HttpPost("enviar/{idFactura}")]
        public async Task<IActionResult> EnviarFactura(int idFactura)
        {
            try
            {
                // 1. Obtener factura con detalles y cliente
                var factura = await _facturaRepository.GetFacturaWithDetailsAsync(idFactura);
                if (factura == null) return NotFound("Factura no encontrada");

                // 2. Generar XML
                var xmlBytes = _facturaXmlService.GenerarXmlBytes(factura);

                // 3. Obtener certificado activo
                var certificado = await _certificadoService.GetCertificadoActivoAsync();
                if (certificado == null) return BadRequest("No hay certificado digital activo configurado.");
                
                // 4. Firmar XML
                var certX509 = await _certificadoService.CargarCertificadoX509Async(certificado.Id_Cert);
                var xmlFirmado = _firmaElectronicaService.FirmarXmlSri(xmlBytes, certX509);

                // 5. Enviar a SRI Recepción
                var respuestaRecepcion = await _sriRecepcionClient.EnviarComprobanteAsync(xmlFirmado);

                // 6. Retornar respuesta
                return Ok(new { stage = "Recepcion", response = respuestaRecepcion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.ToString() });
            }
        }

        [HttpGet("autorizar/{claveAcceso}")]
        public async Task<IActionResult> AutorizarFactura(string claveAcceso)
        {
             try
            {
                var resultado = await _sriAutorizacionClient.AutorizarAsync(claveAcceso);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
