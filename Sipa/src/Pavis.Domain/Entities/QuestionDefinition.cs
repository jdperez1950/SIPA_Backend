using Pavis.Domain.Enums;
using Pavis.Domain.ValueObjects;

namespace Pavis.Domain.Entities;

public class QuestionDefinition : BaseEntity
{
    public string Key { get; private set; } = string.Empty;
    public QuestionAxis AxisId { get; private set; }
    public int Order { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public string? HelpText { get; private set; }
    public QuestionControlType ControlType { get; private set; }
    public bool RequiresEvidence { get; private set; }
    public EvidenceConfig EvidenceConfig { get; private set; } = new();
    public List<QuestionOption> Options { get; private set; } = new();
    public List<QuestionDependency> Dependencies { get; private set; } = new();

    protected QuestionDefinition() { }

    public QuestionDefinition(
        string key,
        QuestionAxis axisId,
        int order,
        string text,
        QuestionControlType controlType,
        bool requiresEvidence = false)
    {
        Key = key;
        AxisId = axisId;
        Order = order;
        Text = text;
        ControlType = controlType;
        RequiresEvidence = requiresEvidence;
    }

    public void SetHelpText(string? helpText)
    {
        HelpText = helpText;
    }

    public void SetEvidenceConfig(EvidenceConfig config)
    {
        EvidenceConfig = config;
    }

    public void AddOption(string label, string value)
    {
        Options.Add(new QuestionOption { Label = label, Value = value });
    }

    public void AddDependency(QuestionDependency dependency)
    {
        Dependencies.Add(dependency);
    }
}

public class QuestionOption
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
