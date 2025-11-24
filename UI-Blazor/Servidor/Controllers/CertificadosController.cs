using Core.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace UI_Blazor.Servidor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificadosController : ControllerBase
    {
        private readonly ICertificadoService _certificadoService;

        public CertificadosController(ICertificadoService certificadoService)
        {
            _certificadoService = certificadoService;
        }

        // GET: api/Certificados
        [HttpGet]
        public async Task<ActionResult<List<CertificadoDigital>>> GetCertificados()
        {
            var certificados = await _certificadoService.GetAllCertificadosAsync();
            return Ok(certificados);
        }

        // GET: api/Certificados/activo
        [HttpGet("activo")]
        public async Task<ActionResult<CertificadoDigital>> GetCertificadoActivo()
        {
            var certificado = await _certificadoService.GetCertificadoActivoAsync();
            
            if (certificado == null)
            {
                return NotFound(new { error = "No hay certificado activo" });
            }

            return Ok(certificado);
        }

        // POST: api/Certificados
        [HttpPost]
        public async Task<ActionResult<CertificadoDigital>> SubirCertificado([FromForm] IFormFile archivo, [FromForm] string password, [FromForm] string nombre)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                {
                    return BadRequest(new { error = "Debe proporcionar un archivo de certificado" });
                }

                if (string.IsNullOrEmpty(password))
                {
                    return BadRequest(new { error = "Debe proporcionar la contraseña del certificado" });
                }

                // Validar extensión
                var extension = Path.GetExtension(archivo.FileName).ToLower();
                if (extension != ".pfx" && extension != ".p12")
                {
                    return BadRequest(new { error = "El archivo debe ser .pfx o .p12" });
                }

                // Leer bytes del archivo
                byte[] archivoBytes;
                using (var ms = new MemoryStream())
                {
                    await archivo.CopyToAsync(ms);
                    archivoBytes = ms.ToArray();
                }

                var certificado = await _certificadoService.SubirCertificadoAsync(archivoBytes, password, nombre);
                return CreatedAtAction(nameof(GetCertificados), new { id = certificado.Id_Cert }, certificado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/Certificados/5/activar
        [HttpPut("{id}/activar")]
        public async Task<IActionResult> ActivarCertificado(int id)
        {
            try
            {
                var resultado = await _certificadoService.ActivarCertificadoAsync(id);
                
                if (!resultado)
                {
                    return NotFound(new { error = "Certificado no encontrado" });
                }

                return Ok(new { mensaje = "Certificado activado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Certificados/5/validar
        [HttpGet("{id}/validar")]
        public async Task<ActionResult<bool>> ValidarCertificado(int id)
        {
            var esValido = await _certificadoService.ValidarCertificadoAsync(id);
            return Ok(new { valido = esValido });
        }

        // DELETE: api/Certificados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCertificado(int id)
        {
            try
            {
                await _certificadoService.EliminarCertificadoAsync(id);
                return Ok(new { mensaje = "Certificado eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
