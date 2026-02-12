namespace Pavis.Application.DTOs.Projects;

public class UpdateProjectRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? ViabilityStatus { get; set; }
    public Guid? AdvisorId { get; set; }
    public List<string>? ActiveAxes { get; set; }
    public List<TechnicalTableMember>? TechnicalTable { get; set; }
    public List<ResponseTeamMember>? ResponseTeam { get; set; }
    public DatesRequest? Dates { get; set; }
}

public class TechnicalTableMember
{
    public string AxisId { get; set; } = string.Empty;
    public Guid AdvisorId { get; set; }
}
