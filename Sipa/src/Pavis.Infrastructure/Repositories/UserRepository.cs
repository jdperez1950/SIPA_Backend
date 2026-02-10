using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Data;

namespace Pavis.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
    {
        return await _dbSet
            .Where(u => u.Role == role && u.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<int> GetTotalByRoleAsync(UserRole role)
    {
        return await _dbSet
            .CountAsync(u => u.Role == role && u.DeletedAt == null);
    }

    public async Task<IEnumerable<User>> SearchAsync(string query, int page, int limit)
    {
        return await _dbSet
            .Where(u => (u.Name.Contains(query) || u.Email.Contains(query)) && u.DeletedAt == null)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }
}
