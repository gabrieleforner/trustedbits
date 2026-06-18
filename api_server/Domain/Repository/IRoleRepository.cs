using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

/// <summary>
/// Repository contract for operations specific to <see cref="RoleEntity"/>.
/// Provides methods for creation, retrieval (untracked), update and deletion.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Creates a new <see cref="RoleEntity"/> in the data store.
    /// </summary>
    /// <param name="scope">The role entity to create.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The created <see cref="RoleEntity"/>.</returns>
    Task<RoleEntity> CreateAsync(RoleEntity scope, CancellationToken ct = default);
    
    // Untracked
    /// <summary>
    /// Retrieves a role by its identifier. Returned entity is expected to be untracked.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The matching <see cref="RoleEntity"/>, or <c>null</c> if not found.</returns>
    Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a paginated set of all roles.
    /// </summary>
    /// <param name="page">Zero-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An enumerable of <see cref="RoleEntity"/> for the requested page.</returns>
    Task<IEnumerable<RoleEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a role by its (normalized) name.
    /// </summary>
    /// <param name="name">The role name to search.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The matching <see cref="RoleEntity"/>, or <c>null</c> if not found.</returns>
    Task<RoleEntity?> GetByNameAsync(string name, CancellationToken ct = default);

    /// <summary>
    /// Performs a contains/term search on role properties returning a paginated set of matches.
    /// </summary>
    /// <param name="term">Search term to match against role fields.</param>
    /// <param name="page">Zero-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>An enumerable of matching <see cref="RoleEntity"/>.</returns>
    Task<IEnumerable<RoleEntity>> GetByContainsAsync(string term, int page, int pageSize, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="scope">The role entity with updated data.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The updated <see cref="RoleEntity"/>.</returns>
    Task<RoleEntity> UpdateAsync(RoleEntity scope, CancellationToken ct = default);

    /// <summary>
    /// Deletes the provided role from the store.
    /// </summary>
    /// <param name="scope">The role entity to delete.</param>
    /// <param name="ct">A cancellation token.</param>
    Task DeleteAsync(RoleEntity scope, CancellationToken ct = default);
}