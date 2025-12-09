using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace api_server.Services;

public interface IRepository<T> where T : class
{
    public Task<T?> Create(T newEntity);
    public Task<T?> Get(Expression<Func<T, bool>> filterExpression);
    public Task<bool> Update(Expression<Func<T, bool>> filterExpression, T updatedEntityData);
    public Task<bool> Delete(Expression<Func<T, bool>> filterExpression);
    public Task<IEnumerable<T>> GetAll();
}

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _entityDbSet;
    private readonly DbContext _dbContext;

    public Repository(DbContext context) 
    {
        _dbContext = context;
        _entityDbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> Create(TEntity newEntity)
    {
        await _entityDbSet.AddAsync(newEntity);
        await _dbContext.SaveChangesAsync();
        return newEntity;
    }

    public async Task<TEntity?> Get(Expression<Func<TEntity, bool>> filterExpression)
    {
        return await _entityDbSet.FirstOrDefaultAsync(filterExpression);
    }

    public async Task<bool> Update(Expression<Func<TEntity, bool>> filterExpression, TEntity updatedEntityData)
    {
        var existingEntity = await _entityDbSet.FirstOrDefaultAsync(filterExpression);
        if (existingEntity == null)
            return false;
        _dbContext.Entry(existingEntity).CurrentValues.SetValues(updatedEntityData);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(Expression<Func<TEntity, bool>> filterExpression)
    {
        var entityToDelete = await _entityDbSet.FirstOrDefaultAsync(filterExpression);
        if (entityToDelete == null)
            return false;
        _entityDbSet.Remove(entityToDelete);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TEntity>> GetAll()
    {
        return await _entityDbSet.ToListAsync();
    }
}