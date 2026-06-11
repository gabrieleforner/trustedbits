using System.Linq.Expressions;

namespace Trustedbits.ApiServer.Domain.Repository;

public interface IGenericRepository<T> where T : class
{
    Task<T> CreateAsync(T entity, CancellationToken ct = default);
    
    // Non-tracking
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<T>> GetMatchingAsync(Expression<Func<T, bool>> predicate, int page, int pageSize, CancellationToken ct = default);

    Task<T> UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
}