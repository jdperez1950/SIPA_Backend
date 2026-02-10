using System.ComponentModel.DataAnnotations;

namespace Pavis.Application.DTOs.Auth;

/// <summary>
/// Request para restablecer la contraseña de un usuario (solo ADMIN)
/// </summary>
public class RestorePasswordRequest
{
    /// <summary>
    /// Email del usuario cuya contraseña se va a restablecer
    /// </summary>
    /// <example>usuario@pavis.com</example>
    [Required(ErrorMessage = "El email del usuario es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Response con la contraseña temporal generada
/// </summary>
public class RestorePasswordResponse
{
    /// <summary>
    /// Email del usuario
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña temporal generada automáticamente
    /// </summary>
    public string TemporaryPassword { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje informativo
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
