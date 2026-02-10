namespace Pavis.Domain.ValueObjects;

public class Evidence
{
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
