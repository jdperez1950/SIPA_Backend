using Pavis.Domain.Entities;
using Pavis.Domain.Enums;

namespace Pavis.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync();
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    Task<int> GetTotalByRoleAsync(UserRole role);
    Task<IEnumerable<User>> SearchAsync(string query, int page, int limit);
}

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByIdentifierAsync(string identifier);
    Task<IEnumerable<User>> GetUsersByOrganizationAsync(Guid organizationId);
}

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByCodeAsync(string code);
    Task<IEnumerable<Project>> GetByAdvisorAsync(Guid advisorId);
    Task<IEnumerable<Project>> GetByOrganizationAsync(Guid organizationId);
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status);
    Task<(IEnumerable<Project> projects, int total)> GetPaginatedAsync(int page, int limit, string? search = null);
}

public interface IQuestionDefinitionRepository : IRepository<QuestionDefinition>
{
    Task<QuestionDefinition?> GetByKeyAsync(string key);
    Task<IEnumerable<QuestionDefinition>> GetByAxisAsync(QuestionAxis axis);
    Task<IEnumerable<QuestionDefinition>> GetAllOrderedAsync();
}

public interface IProjectResponseRepository : IRepository<ProjectResponse>
{
    Task<IEnumerable<ProjectResponse>> GetByProjectAsync(Guid projectId);
    Task<ProjectResponse?> GetByProjectAndQuestionAsync(Guid projectId, string questionKey);
    Task<IEnumerable<ProjectResponse>> GetByEvaluationStatusAsync(EvaluationStatus status);
    Task<IEnumerable<ProjectResponse>> GetByAdvisorAsync(Guid advisorId);
}

public interface IAxisRepository : IRepository<Axis>
{
    Task<Axis?> GetByCodeAsync(string code);
}
