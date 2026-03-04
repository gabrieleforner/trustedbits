using System.Linq.Expressions;

namespace Trustedbits.ApiServer.Data.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    public Task<TEntity> Create(TEntity entity);
    public Task CreateRange(IEnumerable<TEntity> entities);
    
    public Task<TEntity?> Get(TEntity entity);
    public Task<IEnumerable<TEntity?>> Get(Expression<Func<TEntity, bool>> predicate);
    public Task<IEnumerable<TEntity>> GetAll();
    public Task<IEnumerable<TEntity>> GetAll(int pageNumber, int pageSize);
    
    public Task UpdateEntity(TEntity entity);
    
    public Task DeleteEntity(TEntity entity);
    public Task DeleteRange(IEnumerable<TEntity> entities);
    
    public Task<bool> Exists(Expression<Func<TEntity, bool>> predicate);
    public Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
}