using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Data;

namespace Pavis.Infrastructure.Repositories;

public class QuestionDefinitionRepository : Repository<QuestionDefinition>, IQuestionDefinitionRepository
{
    public QuestionDefinitionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<QuestionDefinition?> GetByKeyAsync(string key)
    {
        return await _dbSet
            .Include(q => q.Options)
            .Include(q => q.Dependencies)
            .FirstOrDefaultAsync(q => q.Key == key && q.DeletedAt == null);
    }

    public async Task<IEnumerable<QuestionDefinition>> GetByAxisAsync(QuestionAxis axis)
    {
        return await _dbSet
            .Include(q => q.Options)
            .Include(q => q.Dependencies)
            .Where(q => q.AxisId == axis && q.DeletedAt == null)
            .OrderBy(q => q.Order)
            .ToListAsync();
    }

    public async Task<IEnumerable<QuestionDefinition>> GetAllOrderedAsync()
    {
        return await _dbSet
            .Include(q => q.Options)
            .Include(q => q.Dependencies)
            .Where(q => q.DeletedAt == null)
            .OrderBy(q => q.AxisId)
            .ThenBy(q => q.Order)
            .ToListAsync();
    }
}
