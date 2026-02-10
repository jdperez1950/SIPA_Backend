using Pavis.Domain.Entities;
using Pavis.Domain.Enums;

namespace Pavis.Domain.Interfaces;

public interface IAuthService
{
    Task<(string token, User user)> LoginAndReturnUserAsync(string email, string password);
    Task<User> RegisterAsync(string name, string email, string password, UserRole role);
    Task<bool> ValidateTokenAsync(string token);
    Task<User?> GetUserFromTokenAsync(string token);
    Task<User?> GetUserByEmailAsync(string email);
    Task<(string temporaryPassword, string message)> RestorePasswordAsync(string email);
}
