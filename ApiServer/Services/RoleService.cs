using AutoMapper;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Abstract;
using Trustedbits.ApiServer.Services.Interfaces;

namespace Trustedbits.ApiServer.Services;

public class RoleService : RoleServiceBase
{
    private IRepository<Role> _roleRepository;
    private IMapper _objectMapper;
    private ILogger _logger;

    public RoleService(IRepository<Role> roleRepository, IMapper objectMapper, ILogger logger)
    {
        _roleRepository = roleRepository;
        _objectMapper = objectMapper;
        _logger = logger;
    }

    public override async Task<RoleServiceResult<RoleDto>> CreateRole(Guid tenantId, RoleDto roleDto)
    {
        if (string.IsNullOrWhiteSpace(roleDto.RoleName))
            return new RoleServiceResult<RoleDto>(RoleErrors.RoleInvalidData,
                new ServiceError("ERR_INVALID_ROLE_VALUE", "You must provide a non-blank role name."));
    
        var mappedDto = _objectMapper.Map<RoleDto, Role>(roleDto);
        mappedDto.ParentTenantId = tenantId;
        
        try
        {
            // Verify if there is already a role registered
            var matching = await _roleRepository
                .Get(x => x.Name == roleDto.RoleName && x.ParentTenantId == tenantId);
            if (matching.ToList().Count != 0)
                return new RoleServiceResult<RoleDto>(RoleErrors.RoleAlreadyExists,
                    new ServiceError("ERR_ROLE_ALREADY_EXISTS", "You must provide a unique role name."));
            
            await _roleRepository.Create(mappedDto);
            var mappedResult = _objectMapper.Map<Role, RoleDto>(mappedDto);
            return new RoleServiceResult<RoleDto>(mappedResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);

            return new RoleServiceResult<RoleDto>(RoleErrors.ServerError,
                new ServiceError("INTERNAL_ERROR", "An unknown error occured while trying to create a new role."));
        }
    }
    public override async Task<RoleServiceResult<IEnumerable<RoleDto>>> GetAll(Guid tenantId)
    {
        try
        {
            var matchingRoles = await _roleRepository
                .Get(x => x.ParentTenantId == tenantId);
            var mappedMatching = _objectMapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(matchingRoles);
            return new RoleServiceResult<IEnumerable<RoleDto>>(mappedMatching);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);

            return new RoleServiceResult<IEnumerable<RoleDto>>(RoleErrors.ServerError,
                new ServiceError("INTERNAL_ERROR", "An unknown error occured while trying to retrieve roles."));
        }
    }

    public override async Task<RoleServiceResult<RoleDto>> EditRole(Guid tenantId, string roleName, RoleDto roleDto)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return new RoleServiceResult<RoleDto>(RoleErrors.RoleInvalidData,
                new ServiceError("ERR_INVALID_ROLE_NAME", "You must provide a non-blank role name."));

        try
        {
            // Verify whether the roleName aims to an existing role
            var matchingRole = (await _roleRepository
                .GetTracked(x => x.Name == roleName && x.ParentTenantId == tenantId))
                .FirstOrDefault();
            if (matchingRole == null)
                return new RoleServiceResult<RoleDto>(RoleErrors.RoleNotFound,
                    new ServiceError("ERR_ROLE_NOT_FOUND", $"Role {roleDto.RoleName} does not exist in this tenant."));

            // Verify if the requested new role name is already present in the DB 
            var matchingNameRoles = (await _roleRepository
                    .GetTracked(x => x.Name == roleDto.RoleName && x.ParentTenantId == tenantId))
                    .FirstOrDefault();

            if (matchingNameRoles != null)
                return new RoleServiceResult<RoleDto>(RoleErrors.RoleAlreadyExists,
                    new ServiceError("ERR_ROLE_NAME_ALREADY_EXISTS", "You must provide a new unique role name."));
            
            // Edit the existing role with the new values (if they are not null or empty)
            if(!string.IsNullOrWhiteSpace(roleDto.RoleName))
                matchingRole.Name = roleDto.RoleName;
            if (!string.IsNullOrWhiteSpace(roleDto.RoleDescription))
                matchingRole.RoleDescription = roleDto.RoleDescription;
            
            await _roleRepository.UpdateEntity(matchingRole);
            
            var mappedResult = _objectMapper.Map<Role, RoleDto>(matchingRole);
            return new RoleServiceResult<RoleDto>(mappedResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);

            return new RoleServiceResult<RoleDto>(RoleErrors.ServerError,
                new ServiceError("INTERNAL_ERROR", "An unknown error occured while trying to retrieve roles."));
        }
    }

    public override async Task<RoleServiceResult<bool>> DeleteRole(Guid tenantId, string roleName)
    {
        throw new NotImplementedException();
    }
}