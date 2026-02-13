namespace Pavis.Domain.Interfaces;

/// <summary>
/// Servicio de envío de correos electrónicos
/// Fácil de reemplazar: implementa esta interfaz con SendGrid, AWS SES, o cualquier proveedor
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía un correo electrónico simple
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    
    /// <summary>
    /// Envía correo de restablecimiento de contraseña
    /// </summary>
    Task SendPasswordResetEmailAsync(string to, string userName, string temporaryPassword);
    
    /// <summary>
    /// Envía correo de bienvenida con credenciales para nuevo usuario
    /// </summary>
    Task SendWelcomeEmailAsync(string to, string userName, string temporaryPassword);
    
    /// <summary>
    /// Verifica si el servicio de correo está configurado y disponible
    /// </summary>
    bool IsConfigured { get; }
}
