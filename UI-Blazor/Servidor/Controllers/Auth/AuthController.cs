using Application.DTOs.Auth;
using Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace UI_Blazor.Servidor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var token = await _authService.LoginAsync(request);

                if (token == null)
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                return Ok(new { Token = token, Username = request.Username });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el inicio de sesión para el usuario {Username}", request.Username);
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}
