using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Trustedbits.ApiServer.Data.Repository;

/// <summary>
/// Implementation of the <see cref="IRepository{TEntity}"/> using
/// the Entity Framework core.
/// </summary>
/// <typeparam name="TEntity">Type of the entity the repository will cover</typeparam>
public class DbRepositoryImpl<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    
    /// <summary>
    /// Initialize an instance of the class. It's called automatically
    /// by ASP.NET passing any parameter through Dependency Injection
    /// </summary>
    /// <param name="dbContext">Instance of the application <see cref="DbContext"/> instance</param>
    public DbRepositoryImpl(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }
    
    /// <inheritdoc/>
    public async Task<TEntity> CreateAsync(TEntity entity)
    {
         var newEntity = await _dbSet.AddAsync(entity);
         await _dbContext.SaveChangesAsync();
         
         return newEntity.Entity;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet
            .Where(predicate)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(TEntity entity)
    {
        var updatedEntity = _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}