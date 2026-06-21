using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository.impl;

public class RoleRepositoryImpl : IRoleRepository
{
    private readonly DbSet<RoleEntity> _dbSet;
    private readonly DbContext _context;

    public RoleRepositoryImpl(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<RoleEntity>();
    }

    public async Task<RoleEntity> CreateAsync(RoleEntity role, CancellationToken cancellationToken = default)
    {
        var result = _dbSet.Add(role);
        await _context.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<RoleEntity> UpdateAsync(RoleEntity role, CancellationToken cancellationToken = default)
    {
        var entityState = _context.Entry(role).State;
        switch (entityState)
        {
            case EntityState.Modified:
            {
                // Handle tracked entities update
                await _context.SaveChangesAsync(cancellationToken);
                return role;
            }
            case EntityState.Detached:
            {
                // Handle untracked entities update
                var result = _dbSet.Update(role);
                await _context.SaveChangesAsync(cancellationToken);
                return result.Entity;
            }
            // Theoretically never hit, should always fall in one of the two cases
            default:
                return role;
        }
    }

    public async Task DeleteAsync(RoleEntity role, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RoleEntity?> GetByIdAsync(Guid id, bool isTracked = false, bool eagerLoadScopes = false, bool eagerLoadUsers = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = isTracked ? query.AsTracking() : query.AsNoTracking();      // Handle entity tracking

        if(eagerLoadScopes)
            query = query.Include(r => r.ScopeEntities);
    
        return await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<RoleEntity?> GetByNameAsync(string name, bool isTracked = false, bool eagerLoadScopes = false, bool eagerLoadUsers = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = isTracked ? query.AsTracking() : query.AsNoTracking();      // Handle entity tracking
        return await query.FirstOrDefaultAsync(x => x.NormalizedName.Equals(name.ToLower()), cancellationToken);
    }

    public async Task<IEnumerable<RoleEntity>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoleEntity>> SearchAsync(string term, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var matching = await _dbSet
            .Where(s =>
                s.NormalizedName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                s.Description.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return matching;
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}