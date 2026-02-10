using AutoMapper;
using Pavis.Application.DTOs.Auth;
using Pavis.Application.Interfaces;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
using Pavis.Domain.Interfaces;

namespace Pavis.Application.Services;

public class ApplicationAuthService : IApplicationAuthService
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApplicationAuthService(
        IAuthService authService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _authService = authService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var (token, user) = await _authService.LoginAndReturnUserAsync(request.Email, request.Password);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponse
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        var user = await _authService.RegisterAsync(request.Name, request.Email, request.Password, request.Role);
        await _unitOfWork.SaveChangesAsync();

        var (token, _) = await _authService.LoginAndReturnUserAsync(request.Email, request.Password);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponse
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return await _authService.ValidateTokenAsync(token);
    }

    public async Task<RestorePasswordResponse> RestorePasswordAsync(RestorePasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado o inactivo");
        }

        var (temporaryPassword, message) = await _authService.RestorePasswordAsync(request.Email);
        
        return new RestorePasswordResponse
        {
            Email = user.Email,
            TemporaryPassword = temporaryPassword,
            Message = message
        };
    }
}
