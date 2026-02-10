using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Data;

namespace Pavis.Infrastructure.Repositories;

public class ProjectResponseRepository : Repository<ProjectResponse>, IProjectResponseRepository
{
    public ProjectResponseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProjectResponse>> GetByProjectAsync(Guid projectId)
    {
        return await _dbSet
            .Include(r => r.Evidence)
            .Where(r => r.ProjectId == projectId && r.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<ProjectResponse?> GetByProjectAndQuestionAsync(Guid projectId, string questionKey)
    {
        return await _dbSet
            .Include(r => r.Evidence)
            .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.QuestionKey == questionKey && r.DeletedAt == null);
    }

    public async Task<IEnumerable<ProjectResponse>> GetByEvaluationStatusAsync(EvaluationStatus status)
    {
        return await _dbSet
            .Include(r => r.Evidence)
            .Where(r => r.EvaluationStatus == status && r.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectResponse>> GetByAdvisorAsync(Guid advisorId)
    {
        return await _dbSet
            .Include(r => r.Evidence)
            .Join(
                _context.Projects.Where(p => p.AdvisorId == advisorId),
                response => response.ProjectId,
                project => project.Id,
                (response, project) => response)
            .Where(r => r.DeletedAt == null)
            .ToListAsync();
    }
}
