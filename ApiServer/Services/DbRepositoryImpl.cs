using System.Linq.Expressions;
using ApiServer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Services;

public class DbRepositoryImpl<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public DbRepositoryImpl(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }
    
    public async Task<TEntity> CreateAsync(TEntity entity)
    {
         var newEntity = await _dbSet.AddAsync(entity);
         await _dbContext.SaveChangesAsync();
         
         return newEntity.Entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet
            .Where(predicate)
            .ToListAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        var updatedEntity = _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}