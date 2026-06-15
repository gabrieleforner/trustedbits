using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

public interface IRoleRepository
{
    Task<RoleEntity> CreateAsync(RoleEntity scope, CancellationToken ct = default);
    
    // Untracked
    Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<RoleEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<RoleEntity?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IEnumerable<RoleEntity>> GetByContainsAsync(string term, int page, int pageSize, CancellationToken ct = default);
    
    Task<RoleEntity> UpdateAsync(RoleEntity scope, CancellationToken ct = default);
    Task DeleteAsync(RoleEntity scope, CancellationToken ct = default);
}