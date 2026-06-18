using System.Linq.Expressions;

namespace Trustedbits.ApiServer.Domain.Repository;

/// <summary>
/// Generic repository contract describing basic CRUD and query operations for an entity type.
/// Implementations typically wrap a persistence technology (EF Core, Dapper, etc.).
/// </summary>
/// <typeparam name="T">The entity type the repository operates on.</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Persists a new <paramref name="entity"/> asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The created entity (may include generated keys/fields).</returns>
    Task<T> CreateAsync(T entity, CancellationToken ct = default);
    
    // Non-tracking
    /// <summary>
    /// Returns the first entity matching <paramref name="predicate"/> or <c>null</c> if none found.
    /// This method is intended to return an untracked entity (no change-tracking attached).
    /// </summary>
    /// <param name="predicate">Filter expression used to match an entity.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The first matching entity or <c>null</c>.</returns>
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a paginated list of all entities.
    /// </summary>
    /// <param name="page">Zero-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An enumerable of entities for the requested page.</returns>
    Task<IEnumerable<T>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a paginated list of entities that match the given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">Filter expression used to select matching entities.</param>
    /// <param name="page">Zero-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An enumerable of matching entities for the requested page.</returns>
    Task<IEnumerable<T>> GetMatchingAsync(Expression<Func<T, bool>> predicate, int page, int pageSize, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update (should contain identifying key).</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The updated entity.</returns>
    Task<T> UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Deletes the supplied entity from the underlying store.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="ct">A cancellation token.</param>
    Task DeleteAsync(T entity, CancellationToken ct = default);
}