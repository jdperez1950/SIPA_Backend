namespace Pavis.Application.DTOs.Projects;

public class CreateProjectRequest
{
    public string? Name { get; set; }
    public OrganizationRequest Organization { get; set; } = null!;
    public string Department { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public DatesRequest Dates { get; set; } = null!;
    public List<ResponseTeamMember>? ResponseTeam { get; set; }
}

public class OrganizationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}

public class DatesRequest
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime SubmissionDeadline { get; set; }
}

public class ResponseTeamMember
{
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
}
