namespace Pavis.Application.DTOs.Projects;

/// <summary>
/// Request para agregar un miembro al equipo de respuesta de un proyecto
/// </summary>
public class ProjectTeamMemberRequest
{
    /// <summary>
    /// Email del usuario (obligatorio)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Rol específico en el proyecto (ej: "Líder Técnico", "Apoyo Jurídico")
    /// </summary>
    public string RoleInProject { get; set; } = string.Empty;

    /// <summary>
    /// Número de teléfono (opcional)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Número de documento (opcional)
    /// </summary>
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Tipo de documento (CC, NIT, CE, etc.) (opcional)
    /// </summary>
    public string? DocumentType { get; set; }
}

/// <summary>
/// DTO para representar un miembro del equipo de respuesta
/// </summary>
public class ProjectTeamMemberDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string RoleInProject { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime AssignedAt { get; set; }
}
