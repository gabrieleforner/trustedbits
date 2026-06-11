using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

public interface IScopeRepository
{
    Task<ScopeEntity> CreateAsync(ScopeEntity scope, CancellationToken ct = default);
    
    // Untracked
    Task<ScopeEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ScopeEntity?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<ScopeEntity?> GetByValueAsync(string value, CancellationToken ct = default);
    Task<IEnumerable<ScopeEntity>> GetByNameContainsAsync(string term, int page, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<ScopeEntity>> GetByValueContainsAsync(string term, int page, int pageSize, CancellationToken ct = default);
    
    Task<ScopeEntity> UpdateAsync(ScopeEntity scope, Func<ScopeEntity, ScopeEntity> updateFunc,
        CancellationToken ct =  default);
    Task DeleteAsync(ScopeEntity scope, CancellationToken ct = default);
}