using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Infrastructure.EFCore;

namespace Trustedbits.ApiServer.Domain.Repository.impl;

public class ScopeRepositoryImpl : IScopeRepository
{
    private readonly ServerDbContext _context;
    private readonly DbSet<ScopeEntity> _dbSet;

    public ScopeRepositoryImpl(ServerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<ScopeEntity>();
    }

    public async Task<ScopeEntity> CreateAsync(ScopeEntity scope, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.AddAsync(scope, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<ScopeEntity> UpdateAsync(ScopeEntity scope, CancellationToken cancellationToken = default)
    {
        var entityState = _context.Entry(scope).State;
        switch (entityState)
        {
            case EntityState.Modified:
            {
                // Handle tracked entities update
                await _context.SaveChangesAsync(cancellationToken);
                return scope;
            }
            case EntityState.Detached:
            {
                // Handle untracked entities update
                var result = _dbSet.Update(scope);
                await _context.SaveChangesAsync(cancellationToken);
                return result.Entity;
            }
            // Theoretically never hit, should always fall in one of the two cases
            default:
                return scope;
        }
    }

    public async Task DeleteAsync(ScopeEntity scope, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(scope);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ScopeEntity?> GetByIdAsync(Guid id, bool isTracked = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = isTracked ? query.AsTracking() : query.AsNoTracking();      // Handle entity tracking
        return await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<ScopeEntity?> GetByNameAsync(string name, bool isTracked = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = isTracked ? query.AsTracking() : query.AsNoTracking();      // Handle entity tracking
        return await query.FirstOrDefaultAsync(x => x.NormalizedName.Equals(name.ToLower()), cancellationToken);
    }

    public async Task<ScopeEntity?> GetByValueAsync(string value, bool isTracked = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = isTracked ? query.AsTracking() : query.AsNoTracking();      // Handle entity tracking
        return await query.FirstOrDefaultAsync(x => x.Value.Equals(value.ToLower()), cancellationToken);
    }

    public async Task<IEnumerable<ScopeEntity>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ScopeEntity>> SearchAsync(string term, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var matching = await _dbSet
            .Where(s =>
                s.NormalizedName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                s.Value.Contains(term, StringComparison.OrdinalIgnoreCase) ||
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