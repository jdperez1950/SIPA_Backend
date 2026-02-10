namespace Pavis.Domain.ValueObjects;

public class EvidenceConfig
{
    public int MaxSizeMb { get; set; }
    public List<string> AllowedFormats { get; set; } = new();
    public bool RequiresExpirationDate { get; set; }
}
