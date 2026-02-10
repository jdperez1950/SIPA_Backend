namespace Pavis.Application.DTOs.Auth;

public class LoginAndReturnUserResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}
