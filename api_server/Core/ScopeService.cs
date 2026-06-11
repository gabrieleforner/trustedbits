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
        
        // Check for conflicting unique values
        var nameConflicting = await _repository.GetByNameAsync(scope.ScopeName);
        var valueConflicting = await _repository.GetByNameAsync(scope.ScopeValue);

        // Notify name conflicts
        if (nameConflicting != null)
        {
            var errorDto = new ErrorDto(
                "Scope name already exists",
                new Dictionary<string, object>
                {
                    { "ConflictingAttribute", "ScopeName" },
                    { "ConflictingValue", scope.ScopeName }
                }
            );
            return new ScopeServiceResult<ScopeDto>(errorDto, ErrorType.Conflict);
        }
        // Notify value conflicts
        if (valueConflicting != null)
        {
            var errorDto = new ErrorDto(
                "Scope name already exists",
                new Dictionary<string, object>
                {
                    { "ConflictingAttribute", "ScopeValue" },
                    { "ConflictingValue", scope.ScopeValue }
                }
            );
            return new ScopeServiceResult<ScopeDto>(errorDto, ErrorType.Conflict);
        }

        // Write new scope to the DB
        var mappedScope = _mapper.Map<ScopeEntity>(scope);
        var result = await _repository.CreateAsync(mappedScope);
        
        // Return written scope
        return new ScopeServiceResult<ScopeDto>(_mapper.Map<ScopeDto>(result));
    }

    public async Task<ScopeServiceResult<ScopeDto>> GetScope(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ScopeServiceResult<IAsyncEnumerable<ScopeDto>>> GetAllScopes(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<ScopeServiceResult<IEnumerable<ScopeDto>>> SearchScopes(string term, int page, int size)
    {
        throw new NotImplementedException();
    }

    public async Task<ScopeServiceResult<ScopeDto>> UpdateScope(Guid id, ScopeDto scope)
    {
        throw new NotImplementedException();
    }

    public async Task<ScopeServiceResult<bool>> DeleteScope(Guid id)
    {
        throw new NotImplementedException();
    }
}