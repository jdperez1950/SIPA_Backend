using Pavis.Domain.Enums;

namespace Pavis.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public OrganizationType Type { get; private set; }
    public string Identifier { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Municipality { get; private set; } = string.Empty;
    public string Region { get; private set; } = string.Empty;
    public string? ContactName { get; private set; }
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public OrganizationStatus Status { get; private set; }
    public Guid? UserId { get; private set; }

    protected Organization() { }

    public Organization(
        string name,
        OrganizationType type,
        string identifier,
        string email,
        string municipality,
        string region,
        string? description = null,
        string? address = null,
        OrganizationStatus status = OrganizationStatus.ACTIVE)
    {
        Name = name;
        Type = type;
        Identifier = identifier;
        Email = email;
        Municipality = municipality;
        Region = region;
        Description = description;
        Address = address;
        Status = status;
    }

    public void UpdateContactInfo(string? contactName, string? email)
    {
        ContactName = contactName;
        if (!string.IsNullOrEmpty(email))
        {
            Email = email;
        }
        UpdateTimestamp();
    }

    public void UpdateStatus(OrganizationStatus status)
    {
        Status = status;
        UpdateTimestamp();
    }

    public void LinkUser(Guid userId)
    {
        UserId = userId;
        UpdateTimestamp();
    }

    public bool IsActive => Status == OrganizationStatus.ACTIVE;
}
