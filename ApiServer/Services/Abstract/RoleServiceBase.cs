using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Services.Interfaces;

namespace Trustedbits.ApiServer.Services.Abstract;

public abstract class RoleServiceBase : IRoleService
{
    public abstract Task<RoleServiceResult<RoleDto>> CreateRole(Guid tenantId, RoleDto roleDto);
    public abstract Task<RoleServiceResult<IEnumerable<RoleDto>>> GetAll(Guid tenantId);
    public abstract Task<RoleServiceResult<RoleDto>> EditRole(Guid tenantId, string roleName, RoleDto roleDto);
    public abstract Task<RoleServiceResult<bool>> DeleteRole(Guid tenantId, string roleName);
}