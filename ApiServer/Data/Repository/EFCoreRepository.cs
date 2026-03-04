using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Trustedbits.ApiServer.Data.Repository;

public sealed class EFCoreRepository<TEntity> : AbstractDbRepository<TEntity> where TEntity : class
{
    
    private AppDbContext DbContext;
    private DbSet<TEntity> EntitySet;

    public EFCoreRepository(AppDbContext context)
    {
        DbContext = context;
        EntitySet = DbContext.Set<TEntity>();
    }

    public override async Task<TEntity> Create(TEntity entity)
    {
        await EntitySet.AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    public override async Task CreateRange(IEnumerable<TEntity> entities)
    {
        await EntitySet.AddRangeAsync(entities);
        await DbContext.SaveChangesAsync();
    }

    public override async Task<TEntity?> Get(TEntity entity)
    {
        return await EntitySet.FindAsync(entity);
    }

    public override async Task<IEnumerable<TEntity?>> Get(Expression<Func<TEntity, bool>> predicate)
    {
        var matching = await EntitySet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
        return matching;
    }

    public override async Task<IEnumerable<TEntity>> GetAll()
    {
        return await EntitySet
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<IEnumerable<TEntity>> GetAll(int pageNumber, int pageSize)
    {
        var entities = await EntitySet
            .AsNoTracking()
            .ToListAsync();
        return entities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    public override async Task UpdateEntity(TEntity entity)
    {
        EntitySet.Update(entity);
        await DbContext.SaveChangesAsync();
    }

    public override async Task DeleteEntity(TEntity entity)
    {
        EntitySet.Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    public override async Task DeleteRange(IEnumerable<TEntity> entities)
    {
        EntitySet.RemoveRange(entities);
        await DbContext.SaveChangesAsync();
    }
}