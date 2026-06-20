using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Infrastructure.EFCore;

namespace Trustedbits.ApiServer.Domain.Repository.impl;

/// <summary>
/// Concrete implementation of <see cref="IRoleRepository"/> that delegates
/// common persistence operations to a generic repository implementation.
/// </summary>
public class RoleRepositoryImpl : IRoleRepository
{
    private readonly IGenericRepository<RoleEntity> _genericRepository;
    private readonly ServerDbContext _dbContext;

    /// <summary>
    /// Creates a new instance of <see cref="RoleRepositoryImpl"/>.
    /// </summary>
    /// <param name="genericRepository">The generic repository used to perform storage operations.</param>
    /// <param name="dbContext">The EF Core database context for direct queries with includes.</param>
    public RoleRepositoryImpl(IGenericRepository<RoleEntity> genericRepository, ServerDbContext dbContext)
    {
        _genericRepository = genericRepository;
        _dbContext = dbContext;
    }


    /// <inheritdoc />
    public async Task<RoleEntity> CreateAsync(RoleEntity role, CancellationToken ct = default)
    {
        var result = await _genericRepository.CreateAsync(role, ct);
        return result;
    }

    /// <inheritdoc />
    public async Task<RoleEntity?> GetByIdAsync(Guid id, bool isTracked=false, CancellationToken ct = default)
    {
        var query = isTracked
            ? _dbContext.Roles.AsTracking().Include(r => r.ScopeEntities)
            : _dbContext.Roles.AsNoTracking().Include(r => r.ScopeEntities);
        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RoleEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var entities = await _genericRepository.GetAllAsync(page, pageSize, ct);
        return entities;
    }

    /// <inheritdoc />
    public async Task<RoleEntity?> GetByNameAsync(string name, bool isTracked=false, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetFirstOrDefaultAsync(x => x.NormalizedName == name.ToLower(), isTracked, ct);
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
    
    /// <inheritdoc/>
    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _genericRepository.SaveChanges(ct);
    }
}