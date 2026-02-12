using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
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

    public async Task<Organization> GetOrCreateByIdentifierAsync(
        string identifier,
        string name,
        OrganizationType type,
        string email,
        string municipality,
        string region,
        string? description = null,
        string? address = null)
    {
        var existingOrg = await GetByIdentifierAsync(identifier);
        if (existingOrg != null)
        {
            return existingOrg;
        }

        var newOrg = new Organization(
            name,
            type,
            identifier,
            email,
            municipality,
            region,
            description,
            address);

        await _dbSet.AddAsync(newOrg);
        return newOrg;
    }
}
