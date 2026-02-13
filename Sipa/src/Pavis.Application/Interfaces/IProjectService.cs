using Pavis.Application.DTOs.Common;
using Pavis.Application.DTOs.Projects;

namespace Pavis.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectDTO> CreateProjectAsync(CreateProjectRequest request, Guid? userId);
    Task<ProjectDTO> UpdateProjectAsync(UpdateProjectRequest request);
    Task<ProjectDTO?> GetProjectByIdAsync(Guid id);
    Task<PagedResponse<ProjectDTO>> GetProjectsAsync(GetProjectsRequest request, Guid? userId, string? userRole);
    Task<IEnumerable<ProjectTeamMemberDto>> GetProjectTeamAsync(Guid projectId);
}
