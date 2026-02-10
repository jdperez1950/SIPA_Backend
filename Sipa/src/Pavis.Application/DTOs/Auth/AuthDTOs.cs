namespace Pavis.Application.DTOs.Auth;

/// <summary>
/// Request para iniciar sesión en el sistema
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email del usuario
    /// </summary>
    /// <example>admin@pavis.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario
    /// </summary>
    /// <example>Admin123!</example>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Request para registrar un nuevo usuario
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    /// <example>Juan Pérez</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email del usuario
    /// </summary>
    /// <example>juan@pavis.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario (mínimo 8 caracteres, debe incluir mayúsculas, minúsculas y números)
    /// </summary>
    /// <example>Password123!</example>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Rol del usuario (ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION)
    /// </summary>
    /// <example>ADMIN</example>
    public Pavis.Domain.Enums.UserRole Role { get; set; }
}

/// <summary>
/// Response de autenticación exitosa
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Token JWT para autenticación
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Información del usuario autenticado
    /// </summary>
    public UserDto User { get; set; } = null!;

    /// <summary>
    /// Mensaje de respuesta
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Información del usuario
/// </summary>
public class UserDto
{
    /// <summary>
    /// ID único del usuario
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email del usuario
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Rol del usuario (ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION)
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Estado del usuario (ACTIVE, INACTIVE)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Color del avatar para UI
    /// </summary>
    public string? AvatarColor { get; set; }

    /// <summary>
    /// Cantidad de proyectos asignados al usuario
    /// </summary>
    public int ProjectsAssigned { get; set; }
}

/// <summary>
/// Request para validar un token JWT
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Token JWT a validar
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
