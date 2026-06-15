using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

public class RoleRepositoryImpl : IRoleRepository
{
    private readonly IGenericRepository<RoleEntity> _genericRepository;

    public RoleRepositoryImpl(IGenericRepository<RoleEntity> genericRepository)
    {
        _genericRepository = genericRepository;
    }


    public async Task<RoleEntity> CreateAsync(RoleEntity role, CancellationToken ct = default)
    {
        var result = await _genericRepository.CreateAsync(role, ct);
        return result;
    }

    public async Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await _genericRepository.GetFirstOrDefaultAsync( x => x.Id == id, ct);
        return result;
    }

    public async Task<IEnumerable<RoleEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var entities = await _genericRepository.GetAllAsync(page, pageSize, ct);
        return entities;
    }

    public async Task<RoleEntity?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetFirstOrDefaultAsync(x => x.NormalizedName == name.ToLower(), ct);
        return matching;
    }
    
    public async Task<IEnumerable<RoleEntity>> GetByContainsAsync(string term, int page, int pageSize, CancellationToken ct = default)
    {
        var matching = await _genericRepository.GetMatchingAsync(x => 
            x.NormalizedName.Contains(term.ToLower(), StringComparison.InvariantCultureIgnoreCase) ||
            x.Description.Contains(term, StringComparison.InvariantCultureIgnoreCase), page, pageSize, ct);
        return matching;
    }

    public async Task<RoleEntity> UpdateAsync(RoleEntity scope, CancellationToken ct = default)
    {
        var result = await _genericRepository.UpdateAsync(scope, ct);
        return result;
    }

    public async Task DeleteAsync(RoleEntity scope, CancellationToken ct = default)
    {
        await _genericRepository.DeleteAsync(scope, ct);
    }
}