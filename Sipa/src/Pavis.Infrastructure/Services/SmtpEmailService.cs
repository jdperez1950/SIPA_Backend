using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Configurations;

namespace Pavis.Infrastructure.Services;

/// <summary>
/// Servicio de correo SMTP genérico
/// Funciona con MailHog (desarrollo) y cualquier servidor SMTP real (producción)
/// 
/// PARA REEMPLAZAR POR SERVICIO REAL:
/// 1. Crea una nueva clase que implemente IEmailService (ej: SendGridEmailService)
/// 2. Registra la implementación en Program.cs en lugar de esta
/// 3. Actualiza las variables de entorno con las credenciales del servicio real
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public bool IsConfigured => !string.IsNullOrEmpty(_settings.SmtpServer);

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("Email service not configured. Email to {To} was not sent.", to);
            return;
        }

        try
        {
            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                UseDefaultCredentials = string.IsNullOrEmpty(_settings.SmtpUser)
            };

            // Solo autenticar si hay credenciales (MailHog no requiere auth)
            if (!string.IsNullOrEmpty(_settings.SmtpUser))
            {
                client.Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPassword);
            }

            var fromAddress = new MailAddress(_settings.FromEmail, _settings.FromName);
            var toAddress = new MailAddress(to);

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            await client.SendMailAsync(message);
            
            _logger.LogInformation("Email sent successfully to {To} via {Server}:{Port}", 
                to, _settings.SmtpServer, _settings.SmtpPort);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendPasswordResetEmailAsync(string to, string userName, string temporaryPassword)
    {
        var subject = "Restablecimiento de Contraseña - PAVIS";
        
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #3B82F6; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .password {{ background-color: #e8f4f8; padding: 15px; margin: 20px 0; text-align: center; font-size: 24px; font-weight: bold; color: #1e40af; border-radius: 5px; border: 2px dashed #3B82F6; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        .warning {{ color: #dc2626; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Restablecimiento de Contraseña</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>{userName}</strong>,</p>
            <p>Se ha restablecido tu contraseña en el sistema PAVIS.</p>
            
            <p>Tu <strong>contraseña temporal</strong> es:</p>
            <div class='password'>{temporaryPassword}</div>
            
            <p class='warning'>Importante:</p>
            <ul>
                <li>Esta contraseña es temporal y debe ser cambiada en tu próximo inicio de sesión</li>
                <li>No compartas esta contraseña con nadie</li>
                <li>Si no solicitaste este cambio, contacta al administrador inmediatamente</li>
            </ul>
            
            <p>Saludos,<br>Equipo PAVIS</p>
        </div>
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2024 PAVIS - Sistema de Gestión de Proyectos</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(to, subject, body, isHtml: true);
    }

    public async Task SendWelcomeEmailAsync(string to, string userName, string temporaryPassword)
    {
        var subject = "Bienvenido a PAVIS - Credenciales de Acceso";
        
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #3B82F6; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .password {{ background-color: #e8f4f8; padding: 15px; margin: 20px 0; text-align: center; font-size: 24px; font-weight: bold; color: #1e40af; border-radius: 5px; border: 2px dashed #3B82F6; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        .warning {{ color: #dc2626; font-weight: bold; }}
        .success {{ color: #16a34a; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Bienvenido a PAVIS!</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>{userName}</strong>,</p>
            <p class='success'>Tu cuenta ha sido creada exitosamente en el sistema PAVIS.</p>
            
            <p>A continuación encontrarás tus credenciales de acceso:</p>
            
            <p><strong>Email:</strong> {to}</p>
            <p><strong>Contraseña temporal:</strong></p>
            <div class='password'>{temporaryPassword}</div>
            
            <p class='warning'>Importante:</p>
            <ul>
                <li>Esta contraseña es temporal y debe ser cambiada en tu primer inicio de sesión</li>
                <li>No compartas estas credenciales con nadie</li>
                <li>Guarda este correo en un lugar seguro</li>
            </ul>
            
            <p><strong>Próximamente recibirás otro correo confirmando el espacio habilitado para completar la información.</strong></p>
            
            <p>Saludos,<br>Equipo PAVIS</p>
        </div>
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2024 PAVIS - Sistema de Gestión de Proyectos</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(to, subject, body, isHtml: true);
    }
}
