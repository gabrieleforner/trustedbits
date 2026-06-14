using AutoMapper;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Domain.Repository;

namespace Trustedbits.ApiServer.Core;

public class ScopeService : IScopeService
{
    private readonly IMapper _mapper;
    private readonly IScopeRepository _repository;
    private readonly ILogger<ScopeService> _logger;

    public ScopeService(IMapper mapper, IScopeRepository repository, ILogger<ScopeService> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }
 
    public async Task<ScopeServiceResult<ScopeDto>> CreateScope(ScopeDto scope)
    {
        // Validate schema contents
        if (string.IsNullOrWhiteSpace(scope.ScopeName))
        {
            var errorDto = new ErrorDto("Scope name is required");
            return new ScopeServiceResult<ScopeDto>(errorDto, ErrorType.BadRequest);
        }
        if (string.IsNullOrWhiteSpace(scope.ScopeValue))
        {
            var errorDto = new ErrorDto("Scope value is required");
            return new ScopeServiceResult<ScopeDto>(errorDto, ErrorType.BadRequest);
        }
        
        // Check for conflicting unique values (name/value)
        var nameConflicting = await _repository.GetByNameAsync(scope.ScopeName);
        if (nameConflicting != null)
            return ScopeHelpers<ScopeDto>.ConflictError("ScopeName", scope.ScopeName);
        
        var valueConflicting = await _repository.GetByValueAsync(scope.ScopeValue);
        if (valueConflicting != null)
            return ScopeHelpers<ScopeDto>.ConflictError("ScopeValue", scope.ScopeValue);

        // Write new scope to the DB and log
        var mappedScope = _mapper.Map<ScopeEntity>(scope);
        var result = await _repository.CreateAsync(mappedScope);
        _logger.LogInformation("Scope created successfully (ID={id})", result.Id);
        
        // Return written scope in DTO format
        return new ScopeServiceResult<ScopeDto>(_mapper.Map<ScopeDto>(result));
    }

    public async Task<ScopeServiceResult<ScopeDto>> GetScope(Guid id)
    {
        // Check if a DTO has been provided
        if(id == Guid.Empty)
            return ScopeHelpers<ScopeDto>.InvalidScopeIdError();
        
        // Look up the repository, if null return not found error, else return mapped DTO
        var result = await _repository.GetByIdAsync(id);
        if(result == null)
            return ScopeHelpers<ScopeDto>.NotFoundError(id);
        return new ScopeServiceResult<ScopeDto>(_mapper.Map<ScopeDto>(result));
    }

    public async Task<ScopeServiceResult<IAsyncEnumerable<ScopeDto>>> GetAllScopes(int page, int pageSize)
    {
        // Validate paging settings
        var validationError = ScopeHelpers<IAsyncEnumerable<ScopeDto>>.ValidatePagingSettings(page, pageSize);
        if (validationError != null)
            return validationError;

        // Retrieve and map all the scope entries in page
        var scopeEntities = await _repository.GetAllAsync(page, pageSize);
        var mappedScopes = _mapper.Map<IEnumerable<ScopeEntity>>(scopeEntities)
            .Select(entity => _mapper.Map<ScopeDto>(entity))
            .ToAsyncEnumerable();

        // Return DTOs
        return new ScopeServiceResult<IAsyncEnumerable<ScopeDto>>(mappedScopes);
    }

    public async Task<ScopeServiceResult<IEnumerable<ScopeDto>>> SearchScopes(string term, int page, int size)
    {
        // Validate paging settings
        var validationError =  ScopeHelpers<IEnumerable<ScopeDto>>.ValidatePagingSettings(page, size);
        if (validationError != null)
            return validationError;

        // Retrieve from repository and return mapped DTOs
        var matching = _repository.GetByContainsAsync(term, page, size);
        var mappedScopes = _mapper.Map<IEnumerable<ScopeDto>>(matching);
        
        return new ScopeServiceResult<IEnumerable<ScopeDto>>(mappedScopes);
    }

    public async Task<ScopeServiceResult<ScopeDto>> UpdateScope(Guid id, ScopeDto scope) 
    {
        // Validate the GUID of target scope
        if (id == Guid.Empty)
            return ScopeHelpers<ScopeDto>.InvalidScopeIdError();
        
        // Find the target scope, if not found return error
        var updateTarget = await _repository.GetByIdAsync(id);
        if (updateTarget == null)
            return ScopeHelpers<ScopeDto>.NotFoundError(id);
        
        var modifyName  = string.IsNullOrWhiteSpace(scope.ScopeName) != true;
        var modifyValue = string.IsNullOrWhiteSpace(scope.ScopeValue) != true;
        var modifyDesc  = string.IsNullOrWhiteSpace(scope.ScopeDescription) != true;
        var isModified = false;

        if (modifyName)
        {
            // Check if new name conflicts with existing scopes
            var conflicting = await _repository.GetByNameAsync(scope.ScopeName.ToLower());
            if (conflicting != null && conflicting.Id != id)
                return ScopeHelpers<ScopeDto>.ConflictError("ScopeName", scope.ScopeName);

            updateTarget.DisplayName = scope.ScopeName;
            updateTarget.NormalizedName = scope.ScopeName.ToLower();
            isModified = true;
        }
        if (modifyValue)
        {
            // Check if new value conflicts with existing scopes
            var conflicting = await _repository.GetByValueAsync(scope.ScopeValue.ToLower());
            if (conflicting != null && conflicting.Id != id)
                return ScopeHelpers<ScopeDto>.ConflictError("ScopeValue", scope.ScopeValue);

            updateTarget.Value = scope.ScopeValue.ToLower();
            isModified = true;
        }
        if (modifyDesc)
        {
            updateTarget.Description = scope.ScopeDescription;
            isModified = true;
        }

        // Apply update only if at least one field has been modified
        if (isModified)
            await _repository.UpdateAsync(updateTarget);
        
        // Regardless of any update, return the DTO of the scope
        return new ScopeServiceResult<ScopeDto>(_mapper.Map<ScopeDto>(updateTarget));
    }

    public async Task<ScopeServiceResult<bool>> DeleteScope(Guid id)
    {
        throw new NotImplementedException();
    }
}