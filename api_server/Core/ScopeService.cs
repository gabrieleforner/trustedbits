using AutoMapper;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Domain.Repository;

namespace Trustedbits.ApiServer.Core;

/// <inheritdoc/>
public class ScopeService : IScopeService
{
    /// <summary>
    /// Object mapper instance
    /// </summary>
    private readonly IMapper _mapper;
    /// <summary>
    /// Scope-specialized repository instance
    /// </summary>
    private readonly IScopeRepository _repository;

    /// <summary>
    /// Logger (required for audit log events on this resource types)
    /// </summary>
    private readonly ILogger<ScopeService> _logger;

    /// <summary>
    /// Constructor of the service layer. Where dependencies are injected
    /// </summary>
    /// <param name="mapper">Object mapper instance</param>
    /// <param name="repository">Scope repository instance</param>
    /// <param name="logger">Logger instance</param>
    public ScopeService(IMapper mapper, IScopeRepository repository, ILogger<ScopeService> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }
 
    /// <inheritdoc/>
    public async Task<Result<ScopeDto>> Create(ScopeDto scope)
    {
        // Validate schema contents
        if (string.IsNullOrWhiteSpace(scope.ScopeName))
            return ResultHelpers<ScopeDto>.BadRequest("Scope name is required");
        
        if (string.IsNullOrWhiteSpace(scope.ScopeValue))
            return ResultHelpers<ScopeDto>.BadRequest("Scope value is required");
        
        // Check for conflicting unique values (name/value)
        var nameConflicting = await _repository.GetByNameAsync(scope.ScopeName);
        if (nameConflicting != null)
            return ResultHelpers<ScopeDto>.ConflictError("ScopeName", scope.ScopeName);
        
        var valueConflicting = await _repository.GetByValueAsync(scope.ScopeValue);
        if (valueConflicting != null)
            return ResultHelpers<ScopeDto>.ConflictError("ScopeValue", scope.ScopeValue);

        // Write new scope to the DB and log
        var mappedScope = _mapper.Map<ScopeEntity>(scope);
        mappedScope.Id = Guid.Empty;
        var result = await _repository.CreateAsync(mappedScope);
        _logger.LogInformation("Scope created successfully (ID={id})", result.Id);
        
        // Return written scope in DTO format
        return new Result<ScopeDto>(_mapper.Map<ScopeDto>(result));
    }
    
    /// <inheritdoc/>
    public async Task<Result<ScopeDto>> Get(Guid id)
    {
        // Check if a DTO has been provided
        if(id == Guid.Empty)
            return ResultHelpers<ScopeDto>.InvalidIdError();
        
        // Look up the repository, if null return not found error, else return mapped DTO
        var result = await _repository.GetByIdAsync(id);
        if(result == null)
            return ResultHelpers<ScopeDto>.NotFoundError(id);
        return new Result<ScopeDto>(_mapper.Map<ScopeDto>(result));
    }

    /// <inheritdoc/>
    public async Task<Result<IAsyncEnumerable<ScopeDto>>> Get(int page, int pageSize)
    {
        // Validate paging settings
        var validationError = ResultHelpers<IAsyncEnumerable<ScopeDto>>.ValidatePagingSettings(page, pageSize);
        if (validationError != null)
            return validationError;

        // Retrieve and map all the scope entries in page
        var scopeEntities = await _repository.GetAllAsync(page, pageSize);
        var mappedScopes = _mapper.Map<IEnumerable<ScopeEntity>>(scopeEntities)
            .Select(entity => _mapper.Map<ScopeDto>(entity))
            .ToAsyncEnumerable();

        // Return DTOs
        return new Result<IAsyncEnumerable<ScopeDto>>(mappedScopes);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<ScopeDto>>> Search(string term, int page, int size)
    {
        // Validate paging settings
        var validationError =  ResultHelpers<IEnumerable<ScopeDto>>.ValidatePagingSettings(page, size);
        if (validationError != null)
            return validationError;

        // Retrieve from repository and return mapped DTOs
        var matching = await _repository.SearchAsync(term, page, size);
        var mappedScopes = _mapper.Map<IEnumerable<ScopeDto>>(matching);
        
        return new Result<IEnumerable<ScopeDto>>(mappedScopes);
    }
    
    /// <inheritdoc/>
    public async Task<Result<ScopeDto>> Update(Guid id, ScopeDto scope) 
    {
        // Validate the GUID of target scope
        if (id == Guid.Empty)
            return ResultHelpers<ScopeDto>.InvalidIdError();
        
        // Find the target scope, if not found return error
        var updateTarget = await _repository.GetByIdAsync(id, isTracked:true);
        if (updateTarget == null)
            return ResultHelpers<ScopeDto>.NotFoundError(id);
        
        var modifyName  = string.IsNullOrWhiteSpace(scope.ScopeName) != true;
        var modifyValue = string.IsNullOrWhiteSpace(scope.ScopeValue) != true;
        var modifyDesc  = string.IsNullOrWhiteSpace(scope.ScopeDescription) != true;

        if (modifyName)
        {
            // Check if new name conflicts with existing scopes
            var conflicting = await _repository.GetByNameAsync(scope.ScopeName!.ToLower());
            if (conflicting != null && conflicting.Id != id)
                return ResultHelpers<ScopeDto>.ConflictError("ScopeName", scope.ScopeName);

            updateTarget.DisplayName = scope.ScopeName;
            updateTarget.NormalizedName = scope.ScopeName.ToLower();
            _logger.LogInformation("Scope NAME updated successfully (TARGET_ID={id})", id);
        }
        if (modifyValue)
        {
            // Check if new value conflicts with existing scopes
            var conflicting = await _repository.GetByValueAsync(scope.ScopeValue!.ToLower());
            if (conflicting != null && conflicting.Id != id)
                return ResultHelpers<ScopeDto>.ConflictError("ScopeValue", scope.ScopeValue);

            updateTarget.Value = scope.ScopeValue.ToLower();
            _logger.LogInformation("Scope VALUE updated successfully (TARGET_ID={id})", id);
        }
        if (modifyDesc)
        {
            updateTarget.Description = scope.ScopeDescription!;
            _logger.LogInformation("Scope DESCRIPTION updated successfully (TARGET_ID={id})", id);
        }

        // Save edits to the DB
        await _repository.UpdateAsync(updateTarget);
        
        // Regardless of any update, return the DTO of the scope
        return new Result<ScopeDto>(_mapper.Map<ScopeDto>(updateTarget));
    }
    
    /// <inheritdoc/>
    public async Task<Result<bool>> Delete(Guid id)
    {
        // Validate the GUID of target scope
        if (id == Guid.Empty)
            return ResultHelpers<bool>.InvalidIdError();
        
        // Find the target scope, if not found return error
        var deleteTarget = await _repository.GetByIdAsync(id);
        if (deleteTarget == null)
            return ResultHelpers<bool>.NotFoundError(id);
        
        // Delete the scope and log
        await _repository.DeleteAsync(deleteTarget);
        _logger.LogInformation("Scope deleted successfully (ID={id})", id);
        
        // Return success
        return new Result<bool>(true);
    }
}