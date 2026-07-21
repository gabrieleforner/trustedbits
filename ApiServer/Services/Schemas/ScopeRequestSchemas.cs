using MediatR;

namespace Trustedbits.ApiServer.Services.Schemas;

public record CreateScopeRequest : IRequest<Result<CreateScopeResponse>>
{
    public required string Name { get; init; }
    public required string Value { get; init; }
    public required string Description { get; init; }
}