using AutoMapper;
using Pavis.Application.DTOs.Auth;
using Pavis.Application.Interfaces;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
using Pavis.Domain.Interfaces;
using System.Linq;

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

        // Convertir string role a enum UserRole
        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
        {
            throw new ArgumentException($"Rol inválido: {request.Role}. Roles válidos: ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION");
        }

        var user = await _authService.RegisterAsync(request.Name, request.Email, request.Password, userRole);
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

    public async Task<PagedResponse<UserDto>> GetUsersAsync(GetAllUsersRequest request)
    {
        // Obtener todos los usuarios
        var users = await _userRepository.GetAllAsync();
        int total = users.Count();

        // Filtro por búsqueda
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            users = users.Where(u =>
                u.Name.ToLowerInvariant().Contains(search) ||
                u.Email.ToLowerInvariant().Contains(search));
        }

        // Filtro por rol
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (Enum.TryParse<UserRole>(request.Role, true, out var role))
            {
                users = users.Where(u => u.Role == role);
            }
        }

        // Filtro por estado
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<UserStatus>(request.Status, true, out var status))
            {
                users = users.Where(u => u.Status == status);
            }
        }

        // Calcular total después de filtros
        total = users.Count();

        // Aplicar paginación
        users = users
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit);

        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return new PagedResponse<UserDto>
        {
            Data = userDtos,
            Total = total,
            Page = request.Page,
            Limit = request.Limit
        };
    }
}
