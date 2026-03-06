using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Services.Interfaces;

namespace Trustedbits.ApiServer.Services.Abstract;

/// <inheritdoc/>
public abstract class ScopeServiceBase : IScopeService
{
    /// <inheritdoc/>
    public abstract Task<ScopeServiceResult<ScopeDto>> CreateScope(Guid tenantId, ScopeDto scopeDto);

    /// <inheritdoc/>
    public abstract Task<ScopeServiceResult<List<ScopeDto>>> GetScope(Guid tenantId, ScopeQueryDto scopeQueryData);
    
    /// <inheritdoc/>
    public abstract Task<ScopeServiceResult<List<ScopeDto>>> GetAllScopes(Guid tenantId, int page, int pageSize);

    /// <inheritdoc/>
    public abstract Task<ScopeServiceResult<ScopeDto>> EditScope(Guid tenantId, string scopeName, ScopeDto scopeEditData);
    
    /// <inheritdoc/>
    public abstract Task<ScopeServiceResult<bool>> DeleteScope(Guid tenantId, string scopeName);
}