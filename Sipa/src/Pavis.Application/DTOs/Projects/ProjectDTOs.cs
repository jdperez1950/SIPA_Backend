using Pavis.Application.DTOs.Common;
using Pavis.Application.DTOs.Organizations;

namespace Pavis.Application.DTOs.Projects;

public class ProjectDTO
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ViabilityStatus { get; set; } = string.Empty;
    public AdvisorDTO? Advisor { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime SubmissionDeadline { get; set; }
    public DateTime? CorrectionDeadline { get; set; }
    public ProjectProgressDTO Progress { get; set; } = null!;
    public OrganizationDTO Organization { get; set; } = null!;
}

public class AdvisorDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ProjectProgressDTO
{
    public int Technical { get; set; }
    public int Legal { get; set; }
    public int Financial { get; set; }
    public int Social { get; set; }
}

public class GetProjectsRequest
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? ViabilityStatus { get; set; }
}
