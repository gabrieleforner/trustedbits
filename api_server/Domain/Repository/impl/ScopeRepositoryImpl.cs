using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository.impl;

/// <summary>
/// Implementation of <see cref="IScopeRepository"/> that delegates storage operations
/// to a generic repository for <see cref="ScopeEntity"/>.
/// </summary>
/// <param name="genericRepository">The generic repository used to perform persistence operations.</param>
public class ScopeRepositoryImpl(IGenericRepository<ScopeEntity> genericRepository) : IScopeRepository
{
    private readonly IGenericRepository<ScopeEntity> _genericRepository = genericRepository;

    /// <inheritdoc />
    public async Task<ScopeEntity> CreateAsync(ScopeEntity scope, CancellationToken ct = default)
    {
        return await _genericRepository.CreateAsync(scope, ct);
    }

    /// <inheritdoc />
    public async Task<ScopeEntity?> GetByIdAsync(Guid id, bool isTracked=false, CancellationToken ct = default)
    {
        return await _genericRepository.GetFirstOrDefaultAsync(s => s.Id == id, isTracked, ct);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScopeEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var rows = await _genericRepository.GetAllAsync(page, pageSize, ct);
        return rows;
    }

    /// <inheritdoc />
    public async Task<ScopeEntity?> GetByNameAsync(string name, bool isTracked=false, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetFirstOrDefaultAsync(s => s.NormalizedName == name.ToLower(), isTracked, ct);
        return matching;
    }

    /// <inheritdoc />
    public async Task<ScopeEntity?> GetByValueAsync(string value, bool isTracked=false, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetFirstOrDefaultAsync(s => s.Value == value.ToLower(), isTracked, ct);
        return matching;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScopeEntity>> GetByContainsAsync(string term, int page, int pageSize,
        CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetMatchingAsync(s =>
            s.NormalizedName.Contains(term.ToLower()) ||
            s.Value.Contains(term.ToLower()) ||
            s.Description.Contains(term.ToLower()), page, pageSize, ct);
        return matching;
    }

    /// <inheritdoc />
    public async Task<ScopeEntity> UpdateAsync(ScopeEntity scope, CancellationToken ct = default)
    {
        return await _genericRepository.UpdateAsync(scope, ct);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(ScopeEntity scope, CancellationToken ct = default)
    {
        await _genericRepository.DeleteAsync(scope, ct);
    }
    
    /// <inheritdoc/>
    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _genericRepository.SaveChanges(ct);
    }
}