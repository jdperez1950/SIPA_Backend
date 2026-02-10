using Pavis.Domain.Enums;

namespace Pavis.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public string? AvatarColor { get; private set; }
    public int ProjectsAssigned { get; private set; }

    protected User() { }

    public User(string name, string email, string password, UserRole role, UserStatus status = UserStatus.ACTIVE)
    {
        Name = name;
        Email = email;
        PasswordHash = password;
        Role = role;
        Status = status;
        ProjectsAssigned = 0;
    }

    public void UpdateStatus(UserStatus status)
    {
        Status = status;
        UpdateTimestamp();
    }

    public void UpdateAvatarColor(string color)
    {
        AvatarColor = color;
        UpdateTimestamp();
    }

    public void IncrementProjectsAssigned()
    {
        ProjectsAssigned++;
        UpdateTimestamp();
    }

    public void DecrementProjectsAssigned()
    {
        if (ProjectsAssigned > 0)
        {
            ProjectsAssigned--;
            UpdateTimestamp();
        }
    }

    public bool IsActive => Status == UserStatus.ACTIVE;

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdateTimestamp();
    }
}
