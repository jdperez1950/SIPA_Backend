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

    /// <summary>
    /// Obtener usuarios del sistema con paginación y filtros (solo ADMIN)
    /// </summary>
    /// <param name="page">Número de página (default: 1)</param>
    /// <param name="limit">Elementos por página (default: 10)</param>
    /// <param name="role">Filtro por rol (ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION)</param>
    /// <param name="search">Búsqueda en nombre o email</param>
    /// <param name="status">Filtro por estado (ACTIVE, INACTIVE)</param>
    /// <returns>Lista paginada de usuarios</returns>
    /// <response code="200">Usuarios obtenidos exitosamente</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="403">Usuario no tiene permisos de administrador</response>
    /// <response code="500">Error interno del servidor</response>
    /// <remarks>
    /// Ejemplos de uso:
    /// GET /api/auth/users?page=1&limit=10
    /// GET /api/auth/users?role=ASESOR
    /// GET /api/auth/users?search=juan&page=1&limit=20
    /// GET /api/auth/users?status=ACTIVE&role=ADMIN
    /// </remarks>
    [HttpGet("users")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserDto>>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedResponse<UserDto>>>> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? role = null,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        try
        {
            _logger.LogInformation("Retrieving users with filters - Page: {Page}, Limit: {Limit}, Role: {Role}, Search: {Search}, Status: {Status}",
                page, limit, role, search, status);

            var request = new GetAllUsersRequest
            {
                Page = page,
                Limit = limit,
                Role = role,
                Search = search,
                Status = status
            };

            var result = await _authService.GetUsersAsync(request);
            return Ok(ApiResponse<PagedResponse<UserDto>>.Ok(result, "Usuarios obtenidos exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<PagedResponse<UserDto>>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Actualizar información de un usuario (solo ADMIN)
    /// </summary>
    /// <param name="request">Datos del usuario a actualizar</param>
    /// <returns>Usuario actualizado</returns>
    /// <remarks>
    /// Requiere autenticación con rol ADMIN.
    /// Permite actualizar nombre, email, rol y color de avatar.
    /// El email debe ser único en el sistema.
    /// </remarks>
    /// <response code="200">Usuario actualizado exitosamente</response>
    /// <response code="400">Datos inválidos o email duplicado</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="403">Usuario no tiene permisos de administrador</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPatch("users")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserUpdateResponse>>> UpdateUser([FromBody] UpdateUserRequest request)
    {
        try
        {
            var validator = new UpdateUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<UserUpdateResponse>.Fail("Datos inválidos", errors));
            }

            _logger.LogInformation("Update user attempt for ID: {UserId}", request.Id);
            var result = await _authService.UpdateUserAsync(request);
            return Ok(ApiResponse<UserUpdateResponse>.Ok(result, result.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Update user failed for ID: {UserId}", request.Id);
            return BadRequest(ApiResponse<UserUpdateResponse>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return StatusCode(500, ApiResponse<UserUpdateResponse>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Activar o desactivar un usuario (solo ADMIN)
    /// </summary>
    /// <param name="request">ID del usuario y nuevo estado</param>
    /// <returns>Usuario actualizado</returns>
    /// <remarks>
    /// Requiere autenticación con rol ADMIN.
    /// Un usuario INACTIVE no puede iniciar sesión.
    /// El ADMIN no puede desactivarse a sí mismo.
    /// </remarks>
    /// <response code="200">Estado del usuario actualizado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="403">Usuario no tiene permisos de administrador o intenta desactivarse a sí mismo</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPatch("users/status")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<UserUpdateResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserUpdateResponse>>> ToggleUserStatus([FromBody] ToggleUserStatusRequest request)
    {
        try
        {
            var validator = new ToggleUserStatusRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<UserUpdateResponse>.Fail("Datos inválidos", errors));
            }

            // Prevenir que un admin se desactive a sí mismo
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == request.Id.ToString())
            {
                _logger.LogWarning("Admin attempted to deactivate themselves");
                return Forbid("No puedes desactivar tu propia cuenta");
            }

            _logger.LogInformation("Toggle user status for ID: {UserId} to {Status}", request.Id, request.Status);
            var result = await _authService.ToggleUserStatusAsync(request);
            return Ok(ApiResponse<UserUpdateResponse>.Ok(result, result.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Toggle user status failed for ID: {UserId}", request.Id);
            return BadRequest(ApiResponse<UserUpdateResponse>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling user status");
            return StatusCode(500, ApiResponse<UserUpdateResponse>.Fail("Error interno del servidor"));
        }
    }
}
