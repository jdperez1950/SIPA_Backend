using Pavis.Domain.Entities;

namespace Pavis.Domain.Entities;

/// <summary>
/// Miembros del equipo de respuesta asignados a un proyecto
/// Relación N:M entre Project y User con rol específico en el proyecto
/// </summary>
public class ProjectTeamMember : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public string RoleInProject { get; private set; } = string.Empty; // Ej: "Líder Técnico", "Apoyo Jurídico"
    public DateTime AssignedAt { get; private set; }
    
    // Navigation properties
    public Project Project { get; private set; } = null!;
    public User User { get; private set; } = null!;

    protected ProjectTeamMember() { }

    public ProjectTeamMember(Guid projectId, Guid userId, string roleInProject)
    {
        ProjectId = projectId;
        UserId = userId;
        RoleInProject = roleInProject;
        AssignedAt = DateTime.UtcNow;
    }

    public void UpdateRole(string newRole)
    {
        RoleInProject = newRole;
        UpdateTimestamp();
    }
}
