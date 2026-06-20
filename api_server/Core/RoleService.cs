using AutoMapper;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Domain.Repository;

namespace Trustedbits.ApiServer.Core;

/// <summary>
/// Implementation of <c>IRoleService</c>
/// </summary>
/// <see cref="IRoleRepository"/>
public class RoleService : IRoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor for initialize service
    /// </summary>
    /// <param name="logger">Instance of logger (required for audit logging)</param>
    /// <param name="repository">Instance of the <c>IRoleRepository</c></param>
    /// <param name="mapper">Object mapper instance</param>
    public RoleService(ILogger<RoleService> logger, IRoleRepository roleRepository, IScopeRepository scopeRepository, IMapper mapper)
    {
        _logger = logger;
        _roleRepository = roleRepository;
        _scopeRepository = scopeRepository;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<Result<RoleDto>> Create(RoleDto role)
    {
        // Validate required values
        if (string.IsNullOrWhiteSpace(role.RoleName))
            return ResultHelpers<RoleDto>.BadRequest("Role name is required");
        
        // Check conflicts on name
        var nameConflicting = await _roleRepository.GetByNameAsync(role.RoleName);
        if (nameConflicting != null)
            return ResultHelpers<RoleDto>.ConflictError("RoleName", role.RoleName);

        // Append to repository and return DTO
        var mappedEntity = _mapper.Map<RoleEntity>(role);
        mappedEntity.Id = Guid.Empty;
        var result = await _roleRepository.CreateAsync(mappedEntity);
        _logger.LogInformation($"Created {role.RoleName} (ID={result.Id})");       
        
        return new Result<RoleDto>(_mapper.Map<RoleDto>(result));
    }

    /// <inheritdoc/>
    public async Task<Result<RoleDto>> Get(Guid id)
    {
        if (id == Guid.Empty)
            return ResultHelpers<RoleDto>.InvalidIdError();
        
        var result = await _roleRepository.GetByIdAsync(id);
        if (result == null)
            return ResultHelpers<RoleDto>.NotFoundError(id);
        
        return new Result<RoleDto>(_mapper.Map<RoleDto>(result));
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleDto>>> Get(int page, int pageSize)
    {
        var validationError = ResultHelpers<IEnumerable<RoleDto>>.ValidatePagingSettings(page, pageSize);
        if (validationError != null)
            return validationError;

        var entries = await _roleRepository.GetAllAsync(page, pageSize);
        var mapped = _mapper.Map<IEnumerable<RoleDto>>(entries);

        return new Result<IEnumerable<RoleDto>>(mapped);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleDto>>> Search(string term, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(term))
            return ResultHelpers<IEnumerable<RoleDto>>.BadRequest("Search term is required");
        
        var pageValidationError = ResultHelpers<IEnumerable<RoleDto>>.ValidatePagingSettings(page, pageSize);
        if (pageValidationError != null)
            return pageValidationError;
        
        var matching = await _roleRepository.GetByContainsAsync(term, page, pageSize);
        var mapped = _mapper.Map<IEnumerable<RoleDto>>(matching);
        
        return new Result<IEnumerable<RoleDto>>(mapped);
    }

    /// <inheritdoc/>
    public async Task<Result<RoleDto>> Update(Guid id, RoleDto role)
    {
        // Validate the GUID of target role
        if (id == Guid.Empty)
            return ResultHelpers<RoleDto>.InvalidIdError();
        
        // Find the target role, if not found return error
        var updateTarget = await _roleRepository.GetByIdAsync(id);
        if (updateTarget == null)
            return ResultHelpers<RoleDto>.NotFoundError(id);
        
        var modifyName  = string.IsNullOrWhiteSpace(role.RoleName) != true;
        var modifyDesc  = string.IsNullOrWhiteSpace(role.RoleDescription) != true;
        var isModified = false;

        if (modifyName)
        {
            // Check if new name conflicts with existing roles
            var conflicting = await _roleRepository.GetByNameAsync(role.RoleName.ToLower());
            if (conflicting != null && conflicting.Id != id)
                return ResultHelpers<RoleDto>.ConflictError("RoleName", role.RoleName);

            updateTarget.DisplayName = role.RoleName;
            updateTarget.NormalizedName = role.RoleName.ToLower();
            isModified = true;
            _logger.LogInformation("Role NAME updated successfully (TARGET_ID={id})", id);
        }
        if (modifyDesc)
        {
            updateTarget.Description = role.RoleDescription;
            isModified = true;
            _logger.LogInformation("Role DESCRIPTION updated successfully (TARGET_ID={id})", id);
        }

        // Apply update only if at least one field has been modified
        if (isModified)
            await _roleRepository.UpdateAsync(updateTarget);
        
        // Regardless of any update, return the DTO of the scope
        return new Result<RoleDto>(_mapper.Map<RoleDto>(updateTarget));
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> Delete(Guid id)
    {
        // Validate the GUID of target role
        if (id == Guid.Empty)
            return ResultHelpers<bool>.InvalidIdError();
        
        var target = await _roleRepository.GetByIdAsync(id);
        if (target == null)
            return ResultHelpers<bool>.NotFoundError(id);
        
        await _roleRepository.DeleteAsync(target);
        return new Result<bool>(true);
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> AssignScope(Guid roleId, Guid scopeId)
    {
        // Check if role exists, and if so retrieve the tracked target
        var targetRole = await _roleRepository.GetByIdAsync(roleId, true);
        if (targetRole == null)
            return ResultHelpers<bool>.NotFoundError(roleId, "Role not found");
        // Check if scope exists, and if so retrieve the tracked target
        var targetScope = await _scopeRepository.GetByIdAsync(scopeId, true);
        if (targetScope == null)
            return ResultHelpers<bool>.NotFoundError(scopeId, "Scope not found");

        // Check if the scope is already assigned
        if (targetRole.ScopeEntities.Contains(targetScope))
            return new Result<bool>(true);  // It's OK as well, just don't rewrite it
        
        // Add the scope and apply changes
        targetRole.ScopeEntities.Add(targetScope);
        await _roleRepository.SaveChanges();
        
        return new Result<bool>(true);
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> RevokeScope(Guid roleId, Guid scopeId)
    {
        // Check if role exists, and if so retrieve the tracked target
        var targetRole = await _roleRepository.GetByIdAsync(roleId, true);
        if (targetRole == null)
            return ResultHelpers<bool>.NotFoundError(roleId, "Role not found");
        // Check if scope exists, and if so retrieve the tracked target
        var targetScope = await _scopeRepository.GetByIdAsync(scopeId, true);
        if (targetScope == null)
            return ResultHelpers<bool>.NotFoundError(scopeId, "Scope not found");


        var scopeToRemove = targetRole.ScopeEntities.FirstOrDefault(s => s.Id == scopeId);
        if (scopeToRemove == null)
        {
            var errorDetail = new Dictionary<string, string>
            {
                { "ScopeId", scopeId.ToString() },
            };
            return ResultHelpers<bool>.BadRequest("Scope was not assigned", errorDetail);
        }
            

        targetRole.ScopeEntities.Remove(scopeToRemove);
        await _roleRepository.SaveChanges();
        return new Result<bool>(true);
    }
}