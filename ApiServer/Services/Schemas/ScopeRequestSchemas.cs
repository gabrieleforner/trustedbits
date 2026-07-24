using MediatR;

namespace Trustedbits.ApiServer.Services.Schemas;

public record CreateScopeRequest : BaseScopeSchema, IRequest<Result<CreateScopeResponse>> { }

public record DescribeScopeRequest : IRequest<Result<DescribeScopeResponse>>
{
    public required Guid Id { get; init; }
}