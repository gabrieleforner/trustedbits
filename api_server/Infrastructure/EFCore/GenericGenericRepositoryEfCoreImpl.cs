using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Repository;

namespace Trustedbits.ApiServer.Infrastructure.EFCore;

public class GenericGenericRepositoryEfCoreImpl<T> : IGenericRepository<T> where T : class
{
    private readonly ServerDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public GenericGenericRepositoryEfCoreImpl(ServerDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet     = _dbContext.Set<T>();
    }

    public async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        var result = await _dbSet.AddAsync(entity, ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return result.Entity;
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, ct);
    }

    public async Task<IEnumerable<T>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        if(page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var list = await _dbSet
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return list;
    }

    public async Task<IEnumerable<T>> GetMatchingAsync(Expression<Func<T, bool>> predicate, int page, int pageSize, CancellationToken ct = default)
    {
        var matching = await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return matching;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
    }
}