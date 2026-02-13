using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Data;

namespace Pavis.Infrastructure.Repositories;

public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(up => up.UserId == userId && up.DeletedAt == null);
    }

    public async Task<UserProfile?> GetByDocumentNumberAsync(string documentNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(up => up.DocumentNumber == documentNumber && up.DeletedAt == null);
    }
}
