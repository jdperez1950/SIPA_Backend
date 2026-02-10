using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pavis.Application.DTOs.Auth;
using Pavis.Application.DTOs.Common;
using Pavis.Application.Interfaces;
using Pavis.Application.Validators;

namespace Pavis.WebApi.Controllers;

/// <summary>
/// Controlador de autenticación y gestión de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApplicationAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IApplicationAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Iniciar sesión en el sistema
    /// </summary>
    /// <param name="request">Credenciales de acceso</param>
    /// <returns>Token JWT y datos del usuario</returns>
    /// <remarks>
    /// Ejemplo de uso:
    /// {
    ///     "email": "admin@pavis.com",
    ///     "password": "Admin123!"
    /// }
    /// </remarks>
    /// <response code="200">Login exitoso</response>
    /// <response code="401">Credenciales inválidas</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var validator = new LoginRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponse>.Fail("Datos inválidos", errors));
            }

            _logger.LogInformation("Login attempt for email: {Email}", request.Email);
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(result, "Login exitoso"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed for email: {Email}", request.Email);
            return Unauthorized(ApiResponse<AuthResponse>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, ApiResponse<AuthResponse>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Registrar un nuevo usuario en el sistema
    /// </summary>
    /// <param name="request">Datos del usuario a registrar</param>
    /// <returns>Token JWT y datos del usuario creado</returns>
    /// <remarks>
    /// Roles disponibles: ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION
    /// </remarks>
    /// <response code="200">Usuario registrado exitosamente</response>
    /// <response code="400">Datos inválidos o email ya registrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var validator = new RegisterRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponse>.Fail("Datos inválidos", errors));
            }

            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);
            var result = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(result, "Registro exitoso"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed for email: {Email}", request.Email);
            return BadRequest(ApiResponse<AuthResponse>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, ApiResponse<AuthResponse>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Validar si un token JWT es válido
    /// </summary>
    /// <param name="request">Token a validar</param>
    /// <returns>True si el token es válido, False en caso contrario</returns>
    /// <response code="200">Validación exitosa</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> Validate([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var isValid = await _authService.ValidateTokenAsync(request.Token);
            return Ok(ApiResponse<bool>.Ok(isValid, isValid ? "Token válido" : "Token inválido"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return StatusCode(500, ApiResponse<bool>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Restablecer la contraseña de un usuario (solo ADMIN)
    /// </summary>
    /// <param name="request">Email del usuario</param>
    /// <returns>Contraseña temporal generada</returns>
    /// <remarks>
    /// Requiere autenticación con rol ADMIN.
    /// Genera una contraseña temporal aleatoria que debe ser comunicada al usuario.
    /// </remarks>
    /// <response code="200">Contraseña restablecida exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">Usuario no autorizado o inactivo</response>
    /// <response code="403">Usuario no tiene permisos de administrador</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("restore-password")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<RestorePasswordResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RestorePasswordResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<RestorePasswordResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<RestorePasswordResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<RestorePasswordResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RestorePasswordResponse>>> RestorePassword([FromBody] RestorePasswordRequest request)
    {
        try
        {
            var validator = new RestorePasswordRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<RestorePasswordResponse>.Fail("Datos inválidos", errors));
            }

            _logger.LogInformation("Password restore attempt for email: {Email}", request.Email);
            var result = await _authService.RestorePasswordAsync(request);
            return Ok(ApiResponse<RestorePasswordResponse>.Ok(result, "Contraseña restaurada exitosamente"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Password restore unauthorized for email: {Email}", request.Email);
            return Unauthorized(ApiResponse<RestorePasswordResponse>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password restore");
            return StatusCode(500, ApiResponse<RestorePasswordResponse>.Fail("Error interno del servidor"));
        }
    }

}
