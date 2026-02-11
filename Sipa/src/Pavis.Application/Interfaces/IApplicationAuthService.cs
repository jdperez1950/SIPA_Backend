using Pavis.Application.DTOs.Auth;

namespace Pavis.Application.Interfaces;

public interface IApplicationAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<RestorePasswordResponse> RestorePasswordAsync(RestorePasswordRequest request);
    Task<PagedResponse<UserDto>> GetUsersAsync(GetAllUsersRequest request);
}
