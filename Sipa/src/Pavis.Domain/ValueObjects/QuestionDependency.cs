namespace Pavis.Domain.ValueObjects;

public class QuestionDependency
{
    public string DependentOnQuestionId { get; set; } = string.Empty;
    public string TriggerValue { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
