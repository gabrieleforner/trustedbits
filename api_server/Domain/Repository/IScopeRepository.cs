using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

public interface IScopeRepository
{
    // CUD (Create-Update-Delete methods)
    Task<ScopeEntity> CreateAsync(ScopeEntity scope, CancellationToken cancellationToken = default);
    Task<ScopeEntity> UpdateAsync(ScopeEntity scope, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScopeEntity scope, CancellationToken cancellationToken = default);
    
    // Single-entry queries (tracked on-demand)
    Task<ScopeEntity?> GetByIdAsync(Guid id, bool isTracked = false, CancellationToken cancellationToken = default);
    Task<ScopeEntity?> GetByNameAsync(string name, bool isTracked = false, CancellationToken cancellationToken = default);
    Task<ScopeEntity?> GetByValueAsync(string value, bool isTracked = false, CancellationToken cancellationToken = default);
    
    // Paged methods
    Task<IEnumerable<ScopeEntity>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScopeEntity>> SearchAsync(string term, int page, int pageSize,
        CancellationToken cancellationToken = default);
    
    Task SaveChanges(CancellationToken cancellationToken = default);
}