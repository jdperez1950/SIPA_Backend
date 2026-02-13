using AutoMapper;
using Pavis.Application.DTOs.Common;
using Pavis.Application.DTOs.Organizations;
using Pavis.Application.DTOs.Projects;
using Pavis.Application.Interfaces;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
using Pavis.Domain.Interfaces;

namespace Pavis.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IProjectTeamMemberRepository _projectTeamMemberRepository;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public ProjectService(
        IProjectRepository projectRepository,
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IProjectTeamMemberRepository projectTeamMemberRepository,
        IAuthService authService,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _projectTeamMemberRepository = projectTeamMemberRepository;
        _authService = authService;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<ProjectDTO> CreateProjectAsync(CreateProjectRequest request, Guid? userId)
    {
        // Validar fechas
        if (request.Dates.Start >= request.Dates.End)
        {
            throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");
        }

        // Validar que la fecha límite de envío esté entre inicio y fin
        if (request.Dates.SubmissionDeadline <= request.Dates.Start || request.Dates.SubmissionDeadline > request.Dates.End)
        {
            throw new ArgumentException("La fecha límite de envío debe estar entre la fecha de inicio y la fecha de fin");
        }

        // Parsear tipo de organización
        if (!Enum.TryParse<OrganizationType>(request.Organization.Type, true, out var orgType))
        {
            throw new ArgumentException($"Tipo de organización inválido: {request.Organization.Type}");
        }

        // Buscar o crear organización
        var organization = await _organizationRepository.GetOrCreateByIdentifierAsync(
            request.Organization.Identifier,
            request.Organization.Name,
            orgType,
            request.Organization.Email,
            request.Organization.Municipality,
            request.Organization.Region,
            request.Organization.Description,
            request.Organization.Address);

        // Generar código único de proyecto
        string projectCode;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            projectCode = Project.GenerateProjectCode();
            attempts++;

            if (attempts >= maxAttempts)
            {
                throw new InvalidOperationException("No se pudo generar un código único después de varios intentos");
            }
        } while (await _projectRepository.CodeExistsAsync(projectCode));

        // Crear proyecto - Asegurar que las fechas sean UTC
        var startDate = request.Dates.Start.Kind == DateTimeKind.Utc ? request.Dates.Start : DateTime.SpecifyKind(request.Dates.Start, DateTimeKind.Utc);
        var endDate = request.Dates.End.Kind == DateTimeKind.Utc ? request.Dates.End : DateTime.SpecifyKind(request.Dates.End, DateTimeKind.Utc);
        var submissionDeadline = request.Dates.SubmissionDeadline.Kind == DateTimeKind.Utc ? request.Dates.SubmissionDeadline : DateTime.SpecifyKind(request.Dates.SubmissionDeadline, DateTimeKind.Utc);

        var project = new Project(
            projectCode,
            request.Name ?? string.Empty,
            organization.Name,
            request.Municipality,
            request.Department,
            startDate,
            endDate,
            submissionDeadline,
            organization.Id,
            userId,
            ProjectStatus.ACTIVE,
            ViabilityScenario.PRE_HABILITADO);

        await _projectRepository.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        // Procesar equipo de respuesta si se proporciona
        if (request.ResponseTeam != null && request.ResponseTeam.Any())
        {
            await ProcessResponseTeamAsync(project.Id, request.ResponseTeam);
        }

        // Mapear a DTO
        var projectDto = _mapper.Map<ProjectDTO>(project);
        projectDto.Organization = _mapper.Map<OrganizationDTO>(organization);

        return projectDto;
    }

    public async Task<ProjectDTO> UpdateProjectAsync(UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(request.Id);
        if (project == null)
        {
            throw new InvalidOperationException("Proyecto no encontrado");
        }

        // Actualizar fechas si se proporcionan
        if (request.Dates != null)
        {
            var startDate = request.Dates.Start.Kind == DateTimeKind.Utc ? request.Dates.Start : DateTime.SpecifyKind(request.Dates.Start, DateTimeKind.Utc);
            var endDate = request.Dates.End.Kind == DateTimeKind.Utc ? request.Dates.End : DateTime.SpecifyKind(request.Dates.End, DateTimeKind.Utc);
            var submissionDeadline = request.Dates.SubmissionDeadline.Kind == DateTimeKind.Utc ? request.Dates.SubmissionDeadline : DateTime.SpecifyKind(request.Dates.SubmissionDeadline, DateTimeKind.Utc);

            if (startDate >= endDate)
            {
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");
            }

            if (submissionDeadline <= startDate || submissionDeadline > endDate)
            {
                throw new ArgumentException("La fecha límite de envío debe estar entre la fecha de inicio y la fecha de fin");
            }
        }

        // Actualizar nombre si se proporciona
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            project.UpdateName(request.Name);
        }

        // Actualizar estado si se proporciona
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<ProjectStatus>(request.Status, true, out var status))
            {
                project.UpdateStatus(status);
            }
        }

        // Actualizar estado de viabilidad si se proporciona
        if (!string.IsNullOrWhiteSpace(request.ViabilityStatus))
        {
            if (Enum.TryParse<ViabilityScenario>(request.ViabilityStatus, true, out var viabilityStatus))
            {
                project.UpdateViabilityStatus(viabilityStatus);
            }
        }

        // Asignar asesor si se proporciona
        if (request.AdvisorId.HasValue)
        {
            var advisor = await _userRepository.GetByIdAsync(request.AdvisorId.Value);
            if (advisor == null)
            {
                throw new InvalidOperationException("Asesor no encontrado");
            }
            project.AssignAdvisor(request.AdvisorId.Value);
        }

        // Guardar cambios
        await _projectRepository.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProjectDTO>(project);
    }

    public async Task<ProjectDTO?> GetProjectByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return null;
        }

        var projectDto = _mapper.Map<ProjectDTO>(project);

        // Cargar organización
        var organization = await _organizationRepository.GetByIdAsync(project.OrganizationId);
        if (organization != null)
        {
            projectDto.Organization = _mapper.Map<OrganizationDTO>(organization);
        }

        // Cargar asesor si existe
        if (project.AdvisorId.HasValue)
        {
            var advisor = await _userRepository.GetByIdAsync(project.AdvisorId.Value);
            if (advisor != null)
            {
                projectDto.Advisor = new AdvisorDTO
                {
                    Id = advisor.Id,
                    Name = advisor.Name,
                    Email = advisor.Email
                };
            }
        }

        return projectDto;
    }

    public async Task<PagedResponse<ProjectDTO>> GetProjectsAsync(GetProjectsRequest request, Guid? userId, string? userRole)
    {
        // Filtrar por creador si el rol es CONSULTA
        if (userRole == "CONSULTA" && userId.HasValue)
        {
            var (projects, total) = await _projectRepository.GetPaginatedByCreatorAsync(
                request.Page,
                request.Limit,
                request.Search,
                userId.Value);

            var projectDtos = new List<ProjectDTO>();

            foreach (var project in projects)
            {
                var projectDto = _mapper.Map<ProjectDTO>(project);

                // Cargar organización
                var organization = await _organizationRepository.GetByIdAsync(project.OrganizationId);
                if (organization != null)
                {
                    projectDto.Organization = _mapper.Map<OrganizationDTO>(organization);
                }

                // Cargar asesor si existe
                if (project.AdvisorId.HasValue)
                {
                    var advisor = await _userRepository.GetByIdAsync(project.AdvisorId.Value);
                    if (advisor != null)
                    {
                        projectDto.Advisor = new AdvisorDTO
                        {
                            Id = advisor.Id,
                            Name = advisor.Name,
                            Email = advisor.Email
                        };
                    }
                }

                projectDtos.Add(projectDto);
            }

            return new PagedResponse<ProjectDTO>
            {
                Data = projectDtos,
                Total = total,
                Page = request.Page,
                Limit = request.Limit
            };
        }
        else
        {
            // Para ADMIN y otros roles, obtener todos los proyectos
            var (projects, total) = await _projectRepository.GetPaginatedAsync(
                request.Page,
                request.Limit,
                request.Search);

            var projectDtos = new List<ProjectDTO>();

            foreach (var project in projects)
            {
                var projectDto = _mapper.Map<ProjectDTO>(project);

                // Cargar organización
                var organization = await _organizationRepository.GetByIdAsync(project.OrganizationId);
                if (organization != null)
                {
                    projectDto.Organization = _mapper.Map<OrganizationDTO>(organization);
                }

                // Cargar asesor si existe
                if (project.AdvisorId.HasValue)
                {
                    var advisor = await _userRepository.GetByIdAsync(project.AdvisorId.Value);
                    if (advisor != null)
                    {
                        projectDto.Advisor = new AdvisorDTO
                        {
                            Id = advisor.Id,
                            Name = advisor.Name,
                            Email = advisor.Email
                        };
                    }
                }

                projectDtos.Add(projectDto);
            }

            return new PagedResponse<ProjectDTO>
            {
                Data = projectDtos,
                Total = total,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }

    /// <summary>
    /// Procesa el equipo de respuesta: busca usuarios existentes o crea nuevos, y los asigna al proyecto
    /// </summary>
    private async Task ProcessResponseTeamAsync(Guid projectId, List<ResponseTeamMember> teamMembers)
    {
        foreach (var member in teamMembers)
        {
            // Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(member.Email);
            
            if (user == null)
            {
                // Crear nuevo usuario con contraseña temporal
                var temporaryPassword = GenerateRandomPassword();
                
                user = await _authService.RegisterAsync(
                    member.Name,
                    member.Email,
                    temporaryPassword,
                    UserRole.CONSULTA); // Rol base para miembros de equipo
                
                // Crear perfil de usuario
                var userProfile = new UserProfile(
                    user.Id,
                    member.Name,
                    member.DocumentType,
                    member.DocumentNumber,
                    member.Phone);
                
                await _userProfileRepository.AddAsync(userProfile);
                
                // Enviar correo de bienvenida
                if (_emailService.IsConfigured)
                {
                    try
                    {
                        await _emailService.SendWelcomeEmailAsync(user.Email, user.Name, temporaryPassword);
                    }
                    catch (Exception ex)
                    {
                        // Loggear error pero no fallar el proceso
                        Console.WriteLine($"Error al enviar correo de bienvenida a {user.Email}: {ex.Message}");
                    }
                }
            }
            else
            {
                // Verificar si ya existe perfil, si no, crearlo
                var existingProfile = await _userProfileRepository.GetByUserIdAsync(user.Id);
                if (existingProfile == null)
                {
                    var userProfile = new UserProfile(
                        user.Id,
                        member.Name,
                        member.DocumentType,
                        member.DocumentNumber,
                        member.Phone);
                    
                    await _userProfileRepository.AddAsync(userProfile);
                }
            }
            
            // Verificar si ya está asignado al proyecto
            var existingAssignment = await _projectTeamMemberRepository.GetByProjectAndUserAsync(projectId, user.Id);
            if (existingAssignment == null)
            {
                // Crear relación proyecto-equipo
                var projectTeamMember = new ProjectTeamMember(
                    projectId,
                    user.Id,
                    member.RoleInProject);
                
                await _projectTeamMemberRepository.AddAsync(projectTeamMember);
            }
            else
            {
                // Actualizar rol si cambió
                if (existingAssignment.RoleInProject != member.RoleInProject)
                {
                    existingAssignment.UpdateRole(member.RoleInProject);
                    await _projectTeamMemberRepository.UpdateAsync(existingAssignment);
                }
            }
        }
        
        await _unitOfWork.SaveChangesAsync();
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

    public async Task<IEnumerable<ProjectTeamMemberDto>> GetProjectTeamAsync(Guid projectId)
    {
        var teamMembers = await _projectTeamMemberRepository.GetByProjectIdAsync(projectId);
        var result = new List<ProjectTeamMemberDto>();

        foreach (var member in teamMembers)
        {
            var user = await _userRepository.GetByIdAsync(member.UserId);
            if (user == null) continue;
            
            var profile = await _userProfileRepository.GetByUserIdAsync(user.Id);

            result.Add(new ProjectTeamMemberDto
            {
                Id = member.Id,
                UserId = user.Id,
                Email = user.Email,
                Name = profile?.FullName ?? user.Name,
                RoleInProject = member.RoleInProject,
                PhoneNumber = profile?.PhoneNumber,
                DocumentNumber = profile?.DocumentNumber,
                AssignedAt = member.AssignedAt
            });
        }

        return result;
    }
}
