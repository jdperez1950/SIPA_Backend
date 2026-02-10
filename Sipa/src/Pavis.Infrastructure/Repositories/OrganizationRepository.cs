using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Data;

namespace Pavis.Infrastructure.Repositories;

public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Organization?> GetByIdentifierAsync(string identifier)
    {
        return await _dbSet
            .FirstOrDefaultAsync(o => o.Identifier == identifier && o.DeletedAt == null);
    }

    public async Task<IEnumerable<User>> GetUsersByOrganizationAsync(Guid organizationId)
    {
        return await _context.Users
            .Where(u => u.DeletedAt == null)
            .ToListAsync();
    }
}
