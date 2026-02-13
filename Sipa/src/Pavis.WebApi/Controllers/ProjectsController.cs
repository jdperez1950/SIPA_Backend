using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pavis.Application.DTOs.Common;
using Pavis.Application.DTOs.Projects;
using Pavis.Application.Interfaces;
using Pavis.Application.Validators;
using System.Security.Claims;

namespace Pavis.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>
    /// Crear un nuevo proyecto (Flujo Consultante o Admin)
    /// </summary>
    /// <param name="request">Datos del proyecto a crear</param>
    /// <returns>Proyecto creado</returns>
    /// <remarks>
    /// Requiere autenticación con rol CONSULTA o ADMIN.
    /// Crea el proyecto con estado inicial ACTIVE y viabilidad PRE_HABILITADO.
    /// Si la organización ya existe (por NIT), la vincula; si no, la crea.
    /// </remarks>
    /// <response code="201">Proyecto creado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="403">Usuario no tiene permisos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [Authorize(Policy = "ConsultaOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ProjectDTO>>> CreateProject([FromBody] CreateProjectRequest request)
    {
        try
        {
            var validator = new CreateProjectRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ProjectDTO>.Fail("Datos inválidos", errors));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = Guid.TryParse(userIdClaim, out var uid) ? uid : null;

            _logger.LogInformation("Create project attempt for organization: {Organization} by user: {UserId}", request.Organization.Name, userId);
            var result = await _projectService.CreateProjectAsync(request, userId);
            return CreatedAtAction(nameof(GetProjectById), new { id = result.Id }, ApiResponse<ProjectDTO>.Ok(result, "Proyecto creado exitosamente"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Create project failed: {Message}", ex.Message);
            return BadRequest(ApiResponse<ProjectDTO>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return StatusCode(500, ApiResponse<ProjectDTO>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Actualizar un proyecto existente (Flujo Admin)
    /// </summary>
    /// <param name="request">Datos del proyecto a actualizar</param>
    /// <returns>Proyecto actualizado</returns>
    /// <remarks>
    /// Requiere autenticación con rol ADMIN.
    /// Permite actualizar información parcial del proyecto, asignar asesor, cambiar estado, etc.
    /// </remarks>
    /// <response code="200">Proyecto actualizado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="403">Usuario no tiene permisos</response>
    /// <response code="404">Proyecto no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPatch]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ProjectDTO>>> UpdateProject([FromBody] UpdateProjectRequest request)
    {
        try
        {
            var validator = new UpdateProjectRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ProjectDTO>.Fail("Datos inválidos", errors));
            }

            _logger.LogInformation("Update project attempt for ID: {ProjectId}", request.Id);
            var result = await _projectService.UpdateProjectAsync(request);
            return Ok(ApiResponse<ProjectDTO>.Ok(result, "Proyecto actualizado exitosamente"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Update project failed: {Message}", ex.Message);
            return NotFound(ApiResponse<ProjectDTO>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Update project failed: {Message}", ex.Message);
            return BadRequest(ApiResponse<ProjectDTO>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project");
            return StatusCode(500, ApiResponse<ProjectDTO>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Obtener un proyecto por ID
    /// </summary>
    /// <param name="id">ID del proyecto</param>
    /// <returns>Detalles del proyecto</returns>
    /// <response code="200">Proyecto encontrado</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="404">Proyecto no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDTO>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ProjectDTO>>> GetProjectById(Guid id)
    {
        try
        {
            _logger.LogInformation("Get project attempt for ID: {ProjectId}", id);
            var result = await _projectService.GetProjectByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<ProjectDTO>.Fail("Proyecto no encontrado"));
            }

            return Ok(ApiResponse<ProjectDTO>.Ok(result, "Proyecto encontrado"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project");
            return StatusCode(500, ApiResponse<ProjectDTO>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Obtener lista de proyectos con paginación y filtros
    /// </summary>
    /// <param name="page">Número de página (default: 1)</param>
    /// <param name="limit">Elementos por página (default: 10)</param>
    /// <param name="search">Término de búsqueda en código o nombre de organización</param>
    /// <param name="status">Filtro por estado (ACTIVE, SUSPENDED, CERTIFIED, BENEFICIARY)</param>
    /// <param name="viabilityStatus">Filtro por estado de viabilidad (HABILITADO, PRE_HABILITADO, ALTA_POSIBILIDAD, SIN_POSIBILIDAD)</param>
    /// <returns>Lista paginada de proyectos</returns>
    /// <response code="200">Proyectos obtenidos exitosamente</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProjectDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProjectDTO>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProjectDTO>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedResponse<ProjectDTO>>>> GetProjects(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? viabilityStatus = null)
    {
        try
        {
            _logger.LogInformation("Get projects with filters - Page: {Page}, Limit: {Limit}, Search: {Search}, Status: {Status}, ViabilityStatus: {ViabilityStatus}",
                page, limit, search, status, viabilityStatus);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = Guid.TryParse(userIdClaim, out var uid) ? uid : null;

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var request = new GetProjectsRequest
            {
                Page = page,
                Limit = limit,
                Search = search,
                Status = status,
                ViabilityStatus = viabilityStatus
            };

            var result = await _projectService.GetProjectsAsync(request, userId, userRole);
            return Ok(ApiResponse<PagedResponse<ProjectDTO>>.Ok(result, "Proyectos obtenidos exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            return StatusCode(500, ApiResponse<PagedResponse<ProjectDTO>>.Fail("Error interno del servidor"));
        }
    }

    /// <summary>
    /// Obtener el equipo de respuesta asignado a un proyecto
    /// </summary>
    /// <param name="id">ID del proyecto</param>
    /// <returns>Lista de miembros del equipo</returns>
    /// <response code="200">Equipo obtenido exitosamente</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="404">Proyecto no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}/team")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectTeamMemberDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectTeamMemberDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectTeamMemberDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectTeamMemberDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectTeamMemberDto>>>> GetProjectTeam(Guid id)
    {
        try
        {
            _logger.LogInformation("Get project team attempt for project ID: {ProjectId}", id);
            
            // Verificar que el proyecto existe
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound(ApiResponse<IEnumerable<ProjectTeamMemberDto>>.Fail("Proyecto no encontrado"));
            }

            var team = await _projectService.GetProjectTeamAsync(id);
            return Ok(ApiResponse<IEnumerable<ProjectTeamMemberDto>>.Ok(team, "Equipo obtenido exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project team");
            return StatusCode(500, ApiResponse<IEnumerable<ProjectTeamMemberDto>>.Fail("Error interno del servidor"));
        }
    }
}
