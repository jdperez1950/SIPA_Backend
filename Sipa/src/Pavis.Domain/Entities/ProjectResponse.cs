using Pavis.Domain.Enums;
using Pavis.Domain.ValueObjects;

namespace Pavis.Domain.Entities;

public class ProjectResponse : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public string QuestionKey { get; private set; } = string.Empty;
    public string? Value { get; private set; }
    public EvaluationStatus EvaluationStatus { get; private set; }
    public string? EvaluatorObservation { get; private set; }
    public DateTime LastUpdated { get; private set; }
    public Evidence? Evidence { get; private set; }

    protected ProjectResponse() { }

    public ProjectResponse(
        Guid projectId,
        string questionKey,
        string? value = null,
        EvaluationStatus evaluationStatus = EvaluationStatus.PENDING)
    {
        ProjectId = projectId;
        QuestionKey = questionKey;
        Value = value;
        EvaluationStatus = evaluationStatus;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateValue(string? value)
    {
        Value = value;
        LastUpdated = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Validate(string? observation = null)
    {
        EvaluationStatus = EvaluationStatus.VALIDATED;
        EvaluatorObservation = observation;
        LastUpdated = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Return(string observation)
    {
        EvaluationStatus = EvaluationStatus.RETURNED;
        EvaluatorObservation = observation;
        LastUpdated = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void MarkInProcess()
    {
        EvaluationStatus = EvaluationStatus.IN_PROCESS;
        LastUpdated = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void AttachEvidence(Evidence evidence)
    {
        Evidence = evidence;
        LastUpdated = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public bool IsEditable => EvaluationStatus != EvaluationStatus.VALIDATED;
}
