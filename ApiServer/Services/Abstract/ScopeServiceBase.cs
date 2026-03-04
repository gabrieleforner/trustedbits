using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Services.Interfaces;

namespace Trustedbits.ApiServer.Services.Abstract;

public abstract class ScopeServiceBase : IScopeService
{
    public abstract Task<ScopeServiceResult<ScopeDto>> CreateScope(Guid tenantId, ScopeDto scopeDto);
    public abstract Task<ScopeServiceResult<List<ScopeDto>>> GetScope(Guid tenantId, ScopeQueryDto scopeQueryData);
    public abstract Task<ScopeServiceResult<List<ScopeDto>>> GetAllScopes(Guid tenantId);
    public abstract Task<ScopeServiceResult<ScopeDto>> EditScope(Guid tenantId, string scopeName, ScopeDto scopeEditData);
    public abstract Task<ScopeServiceResult<bool>> DeleteScope(Guid tenantId, string scopeName);
}