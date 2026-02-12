using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Data;

namespace Pavis.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Project?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(p => p.AdvisorId != null ? _context.Users.FirstOrDefault(u => u.Id == p.AdvisorId) : null)
            .FirstOrDefaultAsync(p => p.Code == code && p.DeletedAt == null);
    }

    public async Task<IEnumerable<Project>> GetByAdvisorAsync(Guid advisorId)
    {
        return await _dbSet
            .Where(p => p.AdvisorId == advisorId && p.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _dbSet
            .Where(p => p.OrganizationId == organizationId && p.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status)
    {
        return await _dbSet
            .Where(p => p.Status == status && p.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Project> projects, int total)> GetPaginatedAsync(int page, int limit, string? search = null)
    {
        var query = _dbSet.Where(p => p.DeletedAt == null);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Code.Contains(search) || p.OrganizationName.Contains(search));
        }

        var total = await query.CountAsync();
        var projects = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (projects, total);
    }

    public async Task<(IEnumerable<Project> projects, int total)> GetPaginatedByCreatorAsync(int page, int limit, string? search = null, Guid creatorId = default)
    {
        var query = _dbSet.Where(p => p.DeletedAt == null && p.CreatedById == creatorId);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Code.Contains(search) || p.OrganizationName.Contains(search));
        }

        var total = await query.CountAsync();
        var projects = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (projects, total);
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _dbSet
            .AnyAsync(p => p.Code == code && p.DeletedAt == null);
    }
}
