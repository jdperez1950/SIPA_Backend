using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace Pavis.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly JwtConfiguration _jwtConfig;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepository, IOptions<JwtConfiguration> jwtConfig, IEmailService emailService)
    {
        _userRepository = userRepository;
        _jwtConfig = jwtConfig.Value;
        _emailService = emailService;
    }

    public async Task<(string token, User user)> LoginAndReturnUserAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado o inactivo");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Contraseña incorrecta");
        }

        var token = GenerateToken(user);
        return (token, user);
    }

    public async Task<User> RegisterAsync(string name, string email, string password, UserRole role)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(name, email, passwordHash, role, UserStatus.ACTIVE);

        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.SecretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return validatedToken != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> GetUserFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return null;
            }

            return await _userRepository.GetByIdAsync(userId);
        }
        catch
        {
            return null;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<(string temporaryPassword, string message)> RestorePasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado o inactivo");
        }

        // Generar contraseña temporal aleatoria
        var temporaryPassword = GenerateRandomPassword();
        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);
        
        user.UpdatePassword(newPasswordHash);
        await _userRepository.UpdateAsync(user);

        // Enviar correo con la contraseña temporal
        string emailMessage;
        if (_emailService.IsConfigured)
        {
            try
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email, user.Name, temporaryPassword);
                emailMessage = $"Se ha enviado un correo a {user.Email} con la contraseña temporal.";
            }
            catch (Exception)
            {
                // Si falla el envío, igual retornamos la contraseña para que el admin la comunique
                emailMessage = $"No se pudo enviar el correo. La contraseña temporal es: {temporaryPassword}";
            }
        }
        else
        {
            // Modo desarrollo: mostrar contraseña en respuesta
            emailMessage = $"Modo desarrollo - Contraseña temporal: {temporaryPassword}";
        }
        
        return (temporaryPassword, emailMessage);
    }

    private string GenerateRandomPassword()
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";
        
        var random = new Random();
        var password = new System.Text.StringBuilder();
        
        // Asegurar al menos un caracter de cada tipo
        password.Append(uppercase[random.Next(uppercase.Length)]);
        password.Append(lowercase[random.Next(lowercase.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(special[random.Next(special.Length)]);
        
        // Completar con caracteres aleatorios hasta 12 caracteres
        const string allChars = uppercase + lowercase + digits + special;
        for (int i = 4; i < 12; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }
        
        // Mezclar los caracteres
        var array = password.ToString().ToCharArray();
        random.Shuffle(array);
        
        return new string(array);
    }

    private string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.SecretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
