using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

public interface IScopeRepository
{
    Task CreateScopeAsync(Scope scope, CancellationToken cancellationToken = default);
    
    Task<Scope?> GetByIdAsync(Guid scopeId, bool tracking = false, bool loadRoles = false,
        CancellationToken cancellationToken = default);
    Task<Scope?> GetByNameAsync(string scopeName, bool tracking = false, bool loadRoles = false,
        CancellationToken cancellationToken = default);
    Task<Scope?> GetByValueAsync(string scopeValue, bool tracking = false, bool loadRoles = false,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Scope>> GetAll(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Scope>> SearchAsync(string queryTerm, int page, int pageSize, CancellationToken cancellationToken = default);
    
    Task UpdateScopeAsync(Scope target,  Scope values, CancellationToken cancellationToken = default);
    Task DeleteScopeAsync(Scope scope, CancellationToken cancellationToken = default);
}