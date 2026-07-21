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