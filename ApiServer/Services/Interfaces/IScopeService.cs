using Trustedbits.ApiServer.Models;
using Trustedbits.ApiServer.Models.DTOs;

namespace Trustedbits.ApiServer.Services.Interfaces;

public class ScopeServiceResult<TSuccess> : ServiceResult<ScopeErrors, TSuccess>
{
    public ScopeServiceResult(TSuccess value) : base(value) { }
    public ScopeServiceResult(ScopeErrors error, ServiceError errorData) : base(error, errorData) { }
}

// Possible error outcomes from the scope service
public enum ScopeErrors
{
    ScopeAlreadyExists,
    ScopeInvalidData,
    ScopeNotFound,
    ServerError
}

// Formal contract for the scope management service. Since the scope name must be unique, this value is used
// as the main query parameter.
public interface IScopeService
{
    // Create a new scope from scratch
    Task<ScopeServiceResult<ScopeDto>> CreateScope(Guid tenantId, ScopeDto scopeDto);
    
    // Get a single scope querying by a set of search parameters
    Task<ScopeServiceResult<List<ScopeDto>>> GetScope(Guid tenantId, ScopeQueryDto scopeQueryData);

    // Get a list of all scopes (not paginated yet)
    Task<ScopeServiceResult<List<ScopeDto>>> GetAllScopes(Guid tenantId, int page, int pageSize);
    
    // Edit an existing scope (queried by scope name)
    Task<ScopeServiceResult<ScopeDto>> EditScope(Guid tenantId, String scopeName, ScopeDto scopeEditData);
    
    // Delete a scope (queried by scope name)
    Task<ScopeServiceResult<bool>> DeleteScope(Guid tenantId, String scopeName);
}