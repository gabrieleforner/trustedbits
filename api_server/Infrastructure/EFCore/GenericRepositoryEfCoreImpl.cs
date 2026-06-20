using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Repository;

namespace Trustedbits.ApiServer.Infrastructure.EFCore;

/// <summary>
/// EF Core implementation of <see cref="IGenericRepository{T}"/> that provides
/// basic CRUD and paging operations for an entity type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository.</typeparam>
public class GenericGenericRepositoryEfCoreImpl<T> : IGenericRepository<T> where T : class
{
    private readonly ServerDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Creates a new repository instance using the provided <see cref="ServerDbContext"/>.
    /// </summary>
    /// <param name="dbContext">The EF Core database context.</param>
    public GenericGenericRepositoryEfCoreImpl(ServerDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet     = _dbContext.Set<T>();
    }

    /// <summary>
    /// Persists a new entity and returns the stored instance (including generated keys).
    /// </summary>
    /// <param name="entity">The entity to persist.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The created entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
    public async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        var result = await _dbSet.AddAsync(entity, ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return result.Entity;
    }

    /// <summary>
    /// Returns the first element matching the provided predicate or <c>null</c> if none found.
    /// The returned entity is not tracked by the context.
    /// </summary>
    /// <param name="predicate">Filter expression used to find the entity.</param>
    /// <param name="isTracked">Flags whether to enable the entity tracker</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The first matching entity or <c>null</c>.</returns>
    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool isTracked=false, CancellationToken ct = default)
    {
        if (isTracked)
        {
            return await _dbSet
                .AsTracking()
                .FirstOrDefaultAsync(predicate, ct);
        }
        else
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, ct);
        }
    }

    /// <summary>
    /// Retrieves a paginated list of all entities.
    /// </summary>
    /// <param name="page">One-based page index (must be &gt;= 1).</param>
    /// <param name="pageSize">Number of items per page (must be &gt;= 1).</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An enumerable with the entities for the requested page.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="page"/> or <paramref name="pageSize"/> are less than 1.</exception>
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

    /// <summary>
    /// Retrieves a paginated list of entities that match the given predicate.
    /// </summary>
    /// <param name="predicate">Filter expression used to select matching entities.</param>
    /// <param name="page">One-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An enumerable of matching entities for the requested page.</returns>
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

    /// <summary>
    /// Updates the provided entity in the underlying store.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The updated entity instance.</returns>
    public async Task<T> UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync(ct);
        return entity;
    }

    /// <summary>
    /// Deletes the supplied entity from the store.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="ct">A cancellation token.</param>
    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Persists all pending changes made to tracked entities to the underlying data store asynchronously.
    /// This method must be called to commit any modifications from Create, Update, and Delete operations.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}