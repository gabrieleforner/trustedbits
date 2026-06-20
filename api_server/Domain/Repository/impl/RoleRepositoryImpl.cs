using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository.impl;

/// <summary>
/// Concrete implementation of <see cref="IRoleRepository"/> that delegates
/// common persistence operations to a generic repository implementation.
/// </summary>
public class RoleRepositoryImpl : IRoleRepository
{
    private readonly IGenericRepository<RoleEntity> _genericRepository;

    /// <summary>
    /// Creates a new instance of <see cref="RoleRepositoryImpl"/>.
    /// </summary>
    /// <param name="genericRepository">The generic repository used to perform storage operations.</param>
    public RoleRepositoryImpl(IGenericRepository<RoleEntity> genericRepository)
    {
        _genericRepository = genericRepository;
    }


    /// <inheritdoc />
    public async Task<RoleEntity> CreateAsync(RoleEntity role, CancellationToken ct = default)
    {
        var result = await _genericRepository.CreateAsync(role, ct);
        return result;
    }

    /// <inheritdoc />
    public async Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await _genericRepository.GetFirstOrDefaultAsync( x => x.Id == id, ct);
        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RoleEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var entities = await _genericRepository.GetAllAsync(page, pageSize, ct);
        return entities;
    }

    /// <inheritdoc />
    public async Task<RoleEntity?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetFirstOrDefaultAsync(x => x.NormalizedName == name.ToLower(), ct);
        return matching;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<RoleEntity>> GetByContainsAsync(string term, int page, int pageSize, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetMatchingAsync(x => 
            x.NormalizedName.Contains(term.ToLower(), StringComparison.InvariantCultureIgnoreCase) ||
            x.Description.Contains(term, StringComparison.InvariantCultureIgnoreCase), page, pageSize, ct);
        return matching;
    }

    /// <inheritdoc />
    public async Task<RoleEntity> UpdateAsync(RoleEntity scope, CancellationToken ct = default)
    {
        var result = await _genericRepository.UpdateAsync(scope, ct);
        return result;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(RoleEntity scope, CancellationToken ct = default)
    {
        await _genericRepository.DeleteAsync(scope, ct);
    }
}