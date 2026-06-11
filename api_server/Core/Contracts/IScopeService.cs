using Trustedbits.ApiServer.Core.DTO;
using Trustedbits.ApiServer.Core.Patterns;

namespace Trustedbits.ApiServer.Core.Contracts;

/// <summary>
/// Specialization of a generic error for the scope service/Unit of Work
/// </summary>
/// <typeparam name="T"></typeparam>
public class ScopeServiceResult<T> : GenericResult<T>
{
    public ScopeServiceResult(T? data) : base(data) { }
    public ScopeServiceResult(ErrorDto? error, ErrorType errorType) : base(error, errorType) { }
}

/// <summary>
/// Interface defining the use cases for the Scope service/Unit
/// of Work.
/// </summary>
public interface IScopeService
{
    // Create use case
    Task<ScopeServiceResult<ScopeDto>> CreateScope(ScopeDto scope);
    
    // Read use cases
    Task<ScopeServiceResult<ScopeDto>> GetScope(Guid id);
    Task<ScopeServiceResult<IAsyncEnumerable<ScopeDto>>> GetAllScopes(int page, int pageSize);
    Task<ScopeServiceResult<IEnumerable<ScopeDto>>> SearchScopes(string term, int page, int size);
    
    // Update use case
    Task<ScopeServiceResult<ScopeDto>> UpdateScope(Guid id, ScopeDto scope);
    // Delete use case
    Task<ScopeServiceResult<bool>> DeleteScope(Guid id);
}