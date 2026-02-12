using Pavis.Application.DTOs.Auth;
using Pavis.Application.DTOs.Common;

namespace Pavis.Application.Interfaces;

public interface IApplicationAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<RestorePasswordResponse> RestorePasswordAsync(RestorePasswordRequest request);
    Task<PagedResponse<UserDto>> GetUsersAsync(GetAllUsersRequest request);
    Task<UserUpdateResponse> UpdateUserAsync(UpdateUserRequest request);
    Task<UserUpdateResponse> ToggleUserStatusAsync(ToggleUserStatusRequest request);
}
