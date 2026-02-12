using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Data;

namespace Pavis.Infrastructure.Repositories;

public class AxisRepository : Repository<Axis>, IAxisRepository
{
    public AxisRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Axis?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Code == code && a.DeletedAt == null);
    }
}
