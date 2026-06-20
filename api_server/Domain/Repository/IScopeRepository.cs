using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

/// <summary>
/// Repository contract for operations specific to <see cref="ScopeEntity"/>.
/// </summary>
public interface IScopeRepository
{
    /// <summary>
    /// Creates a new <see cref="ScopeEntity"/> in the data store.
    /// </summary>
    /// <param name="scope">The scope entity to create.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The created <see cref="ScopeEntity"/>.</returns>
    Task<ScopeEntity> CreateAsync(ScopeEntity scope, CancellationToken ct = default);
    
    // Untracked
    /// <summary>
    /// Retrieves a scope by its identifier. Returned entity is expected to be untracked.
    /// </summary>
    /// <param name="id">The scope identifier.</param>
    /// <param name="isTracked">Flags wheter to enable entity tracker</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The matching <see cref="ScopeEntity"/>, or <c>null</c> if not found.</returns>
    Task<ScopeEntity?> GetByIdAsync(Guid id, bool isTracked=false, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a paginated set of all scopes.
    /// </summary>
    /// <param name="page">Zero-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An enumerable of <see cref="ScopeEntity"/> for the requested page.</returns>
    Task<IEnumerable<ScopeEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a scope by its (normalized) name.
    /// </summary>
    /// <param name="name">The scope name to search.</param>
    /// <param name="isTracked">Flags whether to enable entity tracker</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The matching <see cref="ScopeEntity"/>, or <c>null</c> if not found.</returns>
    Task<ScopeEntity?> GetByNameAsync(string name, bool isTracked=false, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a scope by its value.
    /// </summary>
    /// <param name="value">The scope value to search.</param>
    /// <param name="isTracked">Flags whether to enable entity tracker</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The matching <see cref="ScopeEntity"/>, or <c>null</c> if not found.</returns>
    Task<ScopeEntity?> GetByValueAsync(string value, bool isTracked=false, CancellationToken ct = default);

    /// <summary>
    /// Performs a contains/term search on scope properties returning a paginated set of matches.
    /// </summary>
    /// <param name="term">Search term to match against scope fields.</param>
    /// <param name="page">Zero-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An enumerable of matching <see cref="ScopeEntity"/>.</returns>
    Task<IEnumerable<ScopeEntity>> GetByContainsAsync(string term, int page, int pageSize, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing scope.
    /// </summary>
    /// <param name="scope">The scope entity with updated data.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The updated <see cref="ScopeEntity"/>.</returns>
    Task<ScopeEntity> UpdateAsync(ScopeEntity scope, CancellationToken ct = default);

    /// <summary>
    /// Deletes the provided scope from the store.
    /// </summary>
    /// <param name="scope">The scope entity to delete.</param>
    /// <param name="ct">A cancellation token.</param>
    Task DeleteAsync(ScopeEntity scope, CancellationToken ct = default);
    
    /// <summary>
    /// Save pending changes made to a Scope.
    /// </summary>
    /// <param name="ct">A cancellation token</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveChanges(CancellationToken ct = default);
}