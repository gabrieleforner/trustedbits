using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Trustedbits.ApiServer.Data.Repository;

public abstract class AbstractDbRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    public abstract Task<TEntity> Create(TEntity entity);
    public abstract Task CreateRange(IEnumerable<TEntity> entities);

    public abstract Task<TEntity?> Get(TEntity entity);
    public abstract Task<IEnumerable<TEntity?>> Get(Expression<Func<TEntity, bool>> predicate);
    public abstract Task<IEnumerable<TEntity>> GetAll();
    public abstract Task<IEnumerable<TEntity>> GetAll(int pageNumber, int pageSize);

    public abstract Task UpdateEntity(TEntity entity);
    public abstract Task DeleteEntity(TEntity entity);
    public abstract Task DeleteRange(IEnumerable<TEntity> entities);

    public async Task<bool> Exists(Expression<Func<TEntity, bool>> predicate)
    {
        var matching = await Get(predicate);
        return matching.ToList().Count > 0;
    }

    public async Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    { 
        return (await Get(predicate))
            .FirstOrDefault();
    }
}