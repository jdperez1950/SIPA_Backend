namespace Pavis.Application.DTOs.Organizations;

public class OrganizationDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? ContactName { get; set; }
    public string Status { get; set; } = string.Empty;
}
