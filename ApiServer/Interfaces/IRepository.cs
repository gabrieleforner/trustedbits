using System.Linq.Expressions;

namespace ApiServer.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    public Task<TEntity> CreateAsync(TEntity entity);
    
    public Task<IEnumerable<TEntity>> GetAllAsync();
    public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    
    public Task UpdateAsync(TEntity entity);
    public Task DeleteAsync(TEntity entity);
}