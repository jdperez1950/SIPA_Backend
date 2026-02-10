using Pavis.Domain.Enums;
using Pavis.Domain.ValueObjects;

namespace Pavis.Domain.Entities;

public class Project : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string OrganizationName { get; private set; } = string.Empty;
    public string Municipality { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public ProjectStatus Status { get; private set; }
    public ViabilityScenario ViabilityStatus { get; private set; }
    public Guid? AdvisorId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime SubmissionDeadline { get; private set; }
    public DateTime? CorrectionDeadline { get; private set; }
    public ProjectProgress Progress { get; private set; }
    public Guid OrganizationId { get; private set; }

    protected Project() 
    {
        Progress = new ProjectProgress();
    }

    public Project(
        string code,
        string organizationName,
        string municipality,
        string state,
        DateTime startDate,
        DateTime endDate,
        DateTime submissionDeadline,
        Guid organizationId,
        ProjectStatus status = ProjectStatus.ACTIVE,
        ViabilityScenario viabilityStatus = ViabilityScenario.PRE_HABILITADO)
    {
        Code = code;
        OrganizationName = organizationName;
        Municipality = municipality;
        State = state;
        StartDate = startDate;
        EndDate = endDate;
        SubmissionDeadline = submissionDeadline;
        OrganizationId = organizationId;
        Status = status;
        ViabilityStatus = viabilityStatus;
        Progress = new ProjectProgress();
    }

    public void AssignAdvisor(Guid advisorId)
    {
        AdvisorId = advisorId;
        UpdateTimestamp();
    }

    public void UpdateStatus(ProjectStatus status)
    {
        Status = status;
        UpdateTimestamp();
    }

    public void UpdateViabilityStatus(ViabilityScenario status)
    {
        ViabilityStatus = status;
        UpdateTimestamp();
    }

    public void SetCorrectionDeadline(DateTime deadline)
    {
        CorrectionDeadline = deadline;
        UpdateTimestamp();
    }

    public void UpdateProgress(int technical, int legal, int financial, int social)
    {
        Progress.Update(technical, legal, financial, social);
        UpdateTimestamp();
    }

    public bool IsActive => Status == ProjectStatus.ACTIVE;
    public bool IsEditable => Status == ProjectStatus.ACTIVE;
}
