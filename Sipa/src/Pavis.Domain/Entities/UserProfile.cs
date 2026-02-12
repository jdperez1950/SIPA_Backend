using Pavis.Domain.Entities;

namespace Pavis.Domain.Entities;

/// <summary>
/// Perfil extendido de usuario con datos demográficos y personales
/// Relación 1:1 con User
/// </summary>
public class UserProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? DocumentType { get; private set; } // CC, NIT, CE, etc.
    public string? DocumentNumber { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? JobTitle { get; private set; } // Cargo en la empresa
    
    // Navigation property
    public User User { get; private set; } = null!;

    protected UserProfile() { }

    public UserProfile(Guid userId, string fullName, string? documentType = null, 
        string? documentNumber = null, string? phoneNumber = null, string? jobTitle = null)
    {
        UserId = userId;
        FullName = fullName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        PhoneNumber = phoneNumber;
        JobTitle = jobTitle;
    }

    public void UpdateProfile(string? fullName = null, string? documentType = null, 
        string? documentNumber = null, string? phoneNumber = null, string? jobTitle = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            FullName = fullName;
        }
        if (!string.IsNullOrWhiteSpace(documentType))
        {
            DocumentType = documentType;
        }
        if (!string.IsNullOrWhiteSpace(documentNumber))
        {
            DocumentNumber = documentNumber;
        }
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            PhoneNumber = phoneNumber;
        }
        if (!string.IsNullOrWhiteSpace(jobTitle))
        {
            JobTitle = jobTitle;
        }
        UpdateTimestamp();
    }
}
