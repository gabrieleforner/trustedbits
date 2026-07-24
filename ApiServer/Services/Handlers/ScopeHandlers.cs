using AutoMapper;
using MediatR;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Domain.Repository;
using Trustedbits.ApiServer.Services.Schemas;

namespace Trustedbits.ApiServer.Services.Handlers;

public class CreateScopeHandler : IRequestHandler<CreateScopeRequest, Result<CreateScopeResponse>>
{
    private readonly IScopeRepository _scopeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateScopeHandler> _logger;

    public CreateScopeHandler(IScopeRepository scopeRepository, IMapper mapper, ILogger<CreateScopeHandler> logger)
    {
        _scopeRepository = scopeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CreateScopeResponse>> Handle(CreateScopeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Trying to create scope {request.Name} - {request.Value}");

            // Check for duplicates on name
            if (await _scopeRepository.ExistsByNameAsync(request.Name, cancellationToken))
            {
                _logger.LogError($"CONFLICT_NAME: {request.Name} is already in use.");
                return new Result<CreateScopeResponse>(
                    new ErrorSchema
                    {
                        Message = $"Scope name {request.Name} is already in use.",
                        Type = ErrorType.AlreadyExists,
                    }
                );
            }

            // Check for duplicates on value
            if (await _scopeRepository.ExistsByValueAsync(request.Value, cancellationToken))
            {
                _logger.LogError($"CONFLICT_VALUE: {request.Value} is already in use.");
                return new Result<CreateScopeResponse>(
                    new ErrorSchema
                    {
                        Message = $"Scope value {request.Value} is already in use.",
                        Type = ErrorType.AlreadyExists,
                    }
                );
            }

            _logger.LogInformation($"No conflicts found, applying write...");

            // Map and write to DB
            var mapped = _mapper.Map<Scope>(request);
            await _scopeRepository.CreateScopeAsync(mapped, cancellationToken);

            // Return response
            var response = new CreateScopeResponse
            {
                Id = mapped.Id,
                Name = mapped.Name,
                Value = mapped.Value,
                Description = mapped.Description
            };
            return new Result<CreateScopeResponse>(response);
        }
        catch (RepositoryException e)
        {
            // Handle DB errors
            _logger.LogError($"Repository Error: {e.Message}");
            return new Result<CreateScopeResponse>(new ErrorSchema
            {
                Type = ErrorType.ServerError,
                Message = "An internal server error occurred."
            });
        }
        catch (ArgumentException e)
        {
            // Handle format validation errors
            return new Result<CreateScopeResponse>(new ErrorSchema
            {
                Type = ErrorType.BadRequest,
                Message = e.Message
            });
        }
    }
}

public class DescribeScopeHandler : IRequestHandler<DescribeScopeRequest, Result<DescribeScopeResponse>>
{
    private readonly IScopeRepository _scopeRepository;
    private readonly ILogger<DescribeScopeHandler> _logger;
    private readonly IMapper _mapper;

    public DescribeScopeHandler(IScopeRepository scopeRepository, ILogger<DescribeScopeHandler> logger, IMapper mapper)
    {
        _scopeRepository = scopeRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<DescribeScopeResponse>> Handle(DescribeScopeRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Trying to describe scope with ID {request.Id}");

        try
        {
            if(request.Id == Guid.Empty)
                throw new ArgumentException("Scope ID cannot be empty.", nameof(request.Id));
            
            
            // Assert that the ID matches an existing scope
            if (await _scopeRepository.ExistsByIdAsync(request.Id, cancellationToken))
            {
                _logger.LogError($"NOT_FOUND: {request.Id} does not map to any scope.");
                return new Result<DescribeScopeResponse>(new ErrorSchema()
                {
                    Type = ErrorType.NotFound,
                    Message = $"No scope with ID {request.Id} was found."
                });
            }
            // If found, return the scope with tracking disabled and roles eager loading disabled
            _logger.LogInformation($"{request.Id} maps to a scope, returning entity.");
            var result = await _scopeRepository.GetByIdAsync(request.Id, false, false, cancellationToken);
            return new Result<DescribeScopeResponse>(_mapper.Map<DescribeScopeResponse>(result));
        }
        catch (ArgumentException e)
        {
            // Handle format validation errors
            return new Result<DescribeScopeResponse>(new ErrorSchema
            {
                Type = ErrorType.BadRequest,
                Message = e.Message
            });
        }
        catch (RepositoryException e)
        {
            // Handle exceptions from the repository.
            _logger.LogError($"Repository Error: {e.Message}");
            return new Result<DescribeScopeResponse>(new ErrorSchema
            {
                Type = ErrorType.ServerError,
                Message = "Internal server error occurred."
            });
        }
    }
}