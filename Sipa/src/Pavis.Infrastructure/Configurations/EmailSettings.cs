namespace Pavis.Infrastructure.Configurations;

/// <summary>
/// Configuración de correo electrónico
/// Compatible con cualquier servidor SMTP (MailHog, SendGrid, AWS SES, Gmail, etc.)
/// </summary>
public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    
    /// <summary>
    /// Servidor SMTP
    /// </summary>
    public string SmtpServer { get; set; } = "localhost";
    
    /// <summary>
    /// Puerto SMTP (25, 587, 465)
    /// </summary>
    public int SmtpPort { get; set; } = 1025;
    
    /// <summary>
    /// Usuario SMTP (opcional para servicios que requieren auth)
    /// </summary>
    public string? SmtpUser { get; set; }
    
    /// <summary>
    /// Contraseña SMTP (opcional para servicios que requieren auth)
    /// </summary>
    public string? SmtpPassword { get; set; }
    
    /// <summary>
    /// Usar SSL/TLS
    /// </summary>
    public bool EnableSsl { get; set; } = false;
    
    /// <summary>
    /// Email del remitente
    /// </summary>
    public string FromEmail { get; set; } = "noreply@pavis.com";
    
    /// <summary>
    /// Nombre del remitente
    /// </summary>
    public string FromName { get; set; } = "PAVIS System";
    
    /// <summary>
    /// Modo desarrollo: no enviar correos reales, solo loggear
    /// </summary>
    public bool DevMode { get; set; } = true;

    /// <summary>
    /// Usar SSL/TLS
    /// </summary>

}
