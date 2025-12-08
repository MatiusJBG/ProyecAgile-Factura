using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services.Facturacion;
using Infrastructure.Services.Sri;
using Core.Interfaces.Facturacion;
using Core.Entities.Facturacion;
using Core.Interfaces.Certificados;

using System.Text.Json;

namespace UI_Blazor.Servidor.Controllers
{
using Core.Interfaces.Common;

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
        private readonly IEmailService _emailService;

        public SriController(
            FacturaXmlService facturaXmlService,
            IFirmaElectronicaService firmaElectronicaService,
            SriRecepcionClient sriRecepcionClient,
            SriAutorizacionClient sriAutorizacionClient,
            IFacturaRepository facturaRepository,
            ICertificadoService certificadoService,
            IRideService rideService,
            IEmailService emailService)
        {
            _facturaXmlService = facturaXmlService;
            _firmaElectronicaService = firmaElectronicaService;
            _sriRecepcionClient = sriRecepcionClient;
            _sriAutorizacionClient = sriAutorizacionClient;
            _facturaRepository = facturaRepository;
            _certificadoService = certificadoService;
            _rideService = rideService;
            _emailService = emailService;
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
                
                // 7. Actualizar Estado Factura en DB
                factura.ClaveAcceso = respuestaRecepcion.ClaveAcceso;
                
                if (respuestaRecepcion.Estado == "RECIBIDA")
                {
                    factura.Estado = Core.Enums.Facturacion.EstadoFactura.Enviada;
                    factura.MensajeError = null;
                }
                else if (respuestaRecepcion.Estado == "DEVUELTA")
                {
                     factura.Estado = Core.Enums.Facturacion.EstadoFactura.Devuelta;
                     factura.MensajeError = respuestaRecepcion.Mensajes;
                }
                else
                {
                    factura.Estado = Core.Enums.Facturacion.EstadoFactura.NoEnviada;
                    factura.MensajeError = "SRI: " + respuestaRecepcion.Estado;
                }
                
                await _facturaRepository.UpdateAsync(factura);

                // 8. Retornar respuesta
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
                
                // Actualizar DB
                var factura = await _facturaRepository.GetByClaveAccesoAsync(claveAcceso);
                if (factura != null)
                {
                    if (resultado.Estado == "AUTORIZADO")
                    {
                        factura.Estado = Core.Enums.Facturacion.EstadoFactura.Autorizada;
                        factura.MensajeError = null;

                        // ENVIAR EMAIL AUTOMÁTICAMENTE
                        if (!string.IsNullOrEmpty(factura.Cliente.Correo))
                        {
                            try
                            {
                                DateTime fechaAuth = DateTime.Now;
                                if (DateTime.TryParse(resultado.FechaAutorizacion, out var parsedDate))
                                {
                                    fechaAuth = parsedDate;
                                }

                                var pdfBytes = _rideService.GenerateRidePdf(factura, claveAcceso, fechaAuth);
                                await _emailService.SendEmailAsync(
                                    factura.Cliente.Correo,
                                    $"Factura Electrónica {factura.Id_Fac} - AUTORIZADA",
                                    $"<p>Estimado {factura.Cliente.Nombre},</p><p>Adjunto encontrará su factura electrónica número {factura.Id_Fac}.</p><p>Estado: AUTORIZADA</p>",
                                    pdfBytes,
                                    $"Factura-{factura.Id_Fac}.pdf"
                                );
                            }
                            catch (Exception ex)
                            {
                                // Loguear error de email pero no fallar la autorización
                                factura.MensajeError += " (Error envío email: " + ex.Message + ")";
                            }
                        }
                    }
                    else
                    {
                        // Si no autorizada, puede ser rechazada o en proceso, asumimos Devuelta/Rechazada
                        factura.Estado = Core.Enums.Facturacion.EstadoFactura.Devuelta;
                        factura.MensajeError = resultado.Mensajes;
                    }
                    await _facturaRepository.UpdateAsync(factura);
                }
                
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
