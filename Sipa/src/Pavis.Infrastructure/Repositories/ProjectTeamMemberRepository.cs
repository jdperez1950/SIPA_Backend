using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Data;

namespace Pavis.Infrastructure.Repositories;

public class ProjectTeamMemberRepository : Repository<ProjectTeamMember>, IProjectTeamMemberRepository
{
    public ProjectTeamMemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProjectTeamMember>> GetByProjectIdAsync(Guid projectId)
    {
        return await _dbSet
            .Where(ptm => ptm.ProjectId == projectId && ptm.DeletedAt == null)
            .OrderBy(ptm => ptm.AssignedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectTeamMember>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(ptm => ptm.UserId == userId && ptm.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<ProjectTeamMember?> GetByProjectAndUserAsync(Guid projectId, Guid userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ptm => ptm.ProjectId == projectId && 
                                        ptm.UserId == userId && 
                                        ptm.DeletedAt == null);
    }

    public async Task<bool> ExistsAsync(Guid projectId, Guid userId)
    {
        return await _dbSet
            .AnyAsync(ptm => ptm.ProjectId == projectId && 
                             ptm.UserId == userId && 
                             ptm.DeletedAt == null);
    }
}
