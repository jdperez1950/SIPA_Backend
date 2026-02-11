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
    /// Rol del usuario (enviar como string/texto)
    /// Valores válidos: ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION
    /// </summary>
    /// <example>ADMIN</example>
    public string Role { get; set; } = string.Empty;
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

/// <summary>
/// Request para obtener usuarios con paginación y filtros
/// </summary>
public class GetAllUsersRequest
{
    /// <summary>
    /// Número de página (default: 1)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Cantidad de elementos por página (default: 10)
    /// </summary>
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Filtro por rol (opcional)
    /// </summary>
    /// <example>ADMIN</example>
    public string? Role { get; set; }

    /// <summary>
    /// Término de búsqueda en nombre o email (opcional)
    /// </summary>
    /// <example>juan</example>
    public string? Search { get; set; }

    /// <summary>
    /// Filtro por estado (opcional)
    /// </summary>
    /// <example>ACTIVE</example>
    public string? Status { get; set; }
}

/// <summary>
/// Response paginado genérico
/// </summary>
/// <typeparam name="T">Tipo de datos</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Lista de datos
    /// </summary>
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Número total de elementos
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Número de página actual
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Cantidad de elementos por página
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)Total / Limit);
}
