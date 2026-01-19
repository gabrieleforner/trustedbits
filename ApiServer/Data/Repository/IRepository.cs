using System.Linq.Expressions;

namespace Trustedbits.ApiServer.Data.Repository;

/// <summary>
/// Interface that describes the actions (exceptions
/// are not documented as they are tightly related to
/// the underlying DB driver (SQL Server, MySQL, ...))
/// </summary>
/// <typeparam name="TEntity">Entity that the repository pattern will cover</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Create asynchronously a new instance of type <c>TEntity</c>
    /// </summary>
    /// <param name="entity">Data that will be used for the new entry on the DB</param>
    /// <returns>The resulting entity after the DB write</returns>
    public Task<TEntity> CreateAsync(TEntity entity);
    
    /// <summary>
    /// Returns all entries of type <c>TEntity</c> as <see cref="IEnumerable{TEntity}"/>
    /// </summary>
    /// <returns><see cref="IEnumerable{TEntity}"/> with all entries</returns>
    public Task<IEnumerable<TEntity>> GetAllAsync();
    
    /// <summary>
    /// Returns all entries that match a LINQ query
    /// </summary>
    /// <param name="predicate">LINQ query</param>
    /// <returns><see cref="IEnumerable{TEntity}"/> with all entries that match LINQ query</returns>
    public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// Update an existing entry
    /// </summary>
    /// <param name="entity">new entity data</param>
    /// <returns></returns>
    public Task UpdateAsync(TEntity entity);
    
    /// <summary>
    /// Delete an existing entry
    /// </summary>
    /// <param name="entity">entity that is going to be deleted</param>
    /// <returns></returns>
    public Task DeleteAsync(TEntity entity);
}