using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services.Facturacion;
using Infrastructure.Services.Sri;
using Core.Interfaces.Facturacion;
using Core.Entities.Facturacion;
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

        private readonly IRideService _rideService;

        public SriController(
            FacturaXmlService facturaXmlService,
            IFirmaElectronicaService firmaElectronicaService,
            SriRecepcionClient sriRecepcionClient,
            SriAutorizacionClient sriAutorizacionClient,
            IFacturaRepository facturaRepository,
            ICertificadoService certificadoService,
            IRideService rideService)
        {
            _facturaXmlService = facturaXmlService;
            _firmaElectronicaService = firmaElectronicaService;
            _sriRecepcionClient = sriRecepcionClient;
            _sriAutorizacionClient = sriAutorizacionClient;
            _facturaRepository = facturaRepository;
            _certificadoService = certificadoService;
            _rideService = rideService;
        }

        [HttpPost("enviar/{idFactura}")]
        public async Task<IActionResult> EnviarFactura(int idFactura)
        {
            try
            {
                // 1. Obtener factura con detalles y cliente
                var factura = await _facturaRepository.GetFacturaWithDetailsAsync(idFactura);
                if (factura == null) return NotFound("Factura no encontrada");

                // 2. Generar XML y obtener Clave
                var resultadoXml = _facturaXmlService.GenerarXmlResult(factura);

                // 3. Obtener certificado activo
                var certificado = await _certificadoService.GetCertificadoActivoAsync();
                if (certificado == null) return BadRequest("No hay certificado digital activo configurado.");
                
                // 4. Firmar XML
                var certX509 = await _certificadoService.CargarCertificadoX509Async(certificado.Id_Cert);
                var xmlFirmado = _firmaElectronicaService.FirmarXmlSri(resultadoXml.XmlBytes, certX509);

                // 5. Enviar a SRI Recepción
                var respuestaRecepcion = await _sriRecepcionClient.EnviarComprobanteAsync(xmlFirmado);

                // 6. Asegurar que la Clave de Acceso esté en la respuesta (usamos la que generamos nosotros)
                if (string.IsNullOrEmpty(respuestaRecepcion.ClaveAcceso))
                {
                    respuestaRecepcion.ClaveAcceso = resultadoXml.ClaveAcceso;
                }

                // 7. Retornar respuesta
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
        [HttpGet("ride/{idFactura}")]
        public async Task<IActionResult> DownloadRide(int idFactura)
        {
            try
            {
                var factura = await _facturaRepository.GetFacturaWithDetailsAsync(idFactura);
                if (factura == null) return NotFound("Factura no encontrada");

                // Get existing signature to extract Access Key and Date if available
                var firma = await _firmaElectronicaService.GetFirmaPorFacturaAsync(idFactura);
                
                string claveAcceso = firma?.Hash_Documento ?? "PENDIENTE"; // Hash field used as temp storage or parse XML? 
                // Wait, Hash_Documento is NOT ClaveAcceso. 
                // We generated ClaveAcceso in FacturaXmlService but didn't save it explicitly in DB except maybe in XML?
                // For now, let's re-generate it or fetch from SriRecepcion persistence if we had it.
                // BETTER: The user's error message showed the ClaveAcceso! It's deterministic.
                
                var xmlResult = _facturaXmlService.GenerarXmlResult(factura);
                claveAcceso = xmlResult.ClaveAcceso;

                DateTime fechaAuth = firma?.Fecha_Firma ?? DateTime.Now;

                var pdfBytes = _rideService.GenerateRidePdf(factura, claveAcceso, fechaAuth);
                
                return File(pdfBytes, "application/pdf", $"RIDE-{claveAcceso}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
