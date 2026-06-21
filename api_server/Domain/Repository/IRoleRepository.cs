using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

public interface IRoleRepository
{
    // CUD (Create-Update-Delete methods)
    Task<RoleEntity> CreateAsync(RoleEntity role, CancellationToken cancellationToken = default);
    Task<RoleEntity> UpdateAsync(RoleEntity role, CancellationToken cancellationToken = default);
    Task DeleteAsync(RoleEntity role, CancellationToken cancellationToken = default);
    
    // Single-entry queries (tracked on-demand, eager load on-demand on both associated scopes and assoictaed users)
    Task<RoleEntity?> GetByIdAsync(Guid id, bool isTracked = false, 
        bool eagerLoadScopes=false,
        bool eagerLoadUsers=false,
        CancellationToken cancellationToken = default);
    Task<RoleEntity?> GetByNameAsync(string name, bool isTracked = false,
        bool eagerLoadScopes=false,
        bool eagerLoadUsers=false,
        CancellationToken cancellationToken = default);

    // Paged methods
    Task<IEnumerable<RoleEntity>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoleEntity>> SearchAsync(string term, int page, int pageSize,
        CancellationToken cancellationToken = default);

    Task SaveChanges(CancellationToken cancellationToken = default);
}