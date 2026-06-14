using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

public class ScopeRepositoryImpl(IGenericRepository<ScopeEntity> genericRepository) : IScopeRepository
{
    private readonly IGenericRepository<ScopeEntity> _genericRepository = genericRepository;

    public async Task<ScopeEntity> CreateAsync(ScopeEntity scope, CancellationToken ct = default)
    {
        return await _genericRepository.CreateAsync(scope, ct);
    }

    public async Task<ScopeEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _genericRepository.GetFirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IEnumerable<ScopeEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var rows = await _genericRepository.GetAllAsync(page, pageSize, ct);
        return rows;
    }

    public async Task<ScopeEntity?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetFirstOrDefaultAsync(s => s.NormalizedName == name.ToLower(), ct);
        return matching;
    }

    public async Task<ScopeEntity?> GetByValueAsync(string value, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetFirstOrDefaultAsync(s => s.Value == value.ToLower(), ct);
        return matching;
    }

    public async Task<IEnumerable<ScopeEntity>> GetByNameContainsAsync(string term, int page, int pageSize, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetMatchingAsync(s => s.NormalizedName.Contains(term.ToLower()), page, pageSize, ct);
        return matching;
    }

    public async Task<IEnumerable<ScopeEntity>> GetByValueContainsAsync(string term, int page, int pageSize, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetMatchingAsync(s => s.Value.Contains(term.ToLower()), page, pageSize, ct);
        return matching;
    }

    public async Task<ScopeEntity> UpdateAsync(ScopeEntity scope, Func<ScopeEntity, ScopeEntity> updateFunc, CancellationToken ct = default)
    {
        return await _genericRepository.UpdateAsync(scope, ct);
    }

    public async Task DeleteAsync(ScopeEntity scope, CancellationToken ct = default)
    {
        await _genericRepository.DeleteAsync(scope, ct);
    }
}