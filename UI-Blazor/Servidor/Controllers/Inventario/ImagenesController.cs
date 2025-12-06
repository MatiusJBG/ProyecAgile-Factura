using Microsoft.AspNetCore.Mvc;

namespace UI_Blazor.Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImagenesController> _logger;

        public ImagenesController(IWebHostEnvironment environment, ILogger<ImagenesController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No se proporcionó ningún archivo");
                }

                // Validar que sea una imagen
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Solo se permiten archivos de imagen (jpg, jpeg, png, gif, webp)");
                }

                // Validar tamaño (máximo 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("El archivo no puede superar los 5MB");
                }

                // Generar nombre único para el archivo
                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "productos");
                
                // Crear directorio si no existe
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Guardar el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Retornar la URL relativa de la imagen
                var imageUrl = $"/images/productos/{fileName}";
                
                return Ok(new { url = imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imagen");
                return StatusCode(500, "Error al procesar la imagen");
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteImage([FromQuery] string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest("No se proporcionó la URL de la imagen");
                }

                // Extraer el nombre del archivo de la URL
                var fileName = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(_environment.WebRootPath, "images", "productos", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { message = "Imagen eliminada correctamente" });
                }

                return NotFound("La imagen no existe");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar imagen");
                return StatusCode(500, "Error al eliminar la imagen");
            }
        }
    }
}
