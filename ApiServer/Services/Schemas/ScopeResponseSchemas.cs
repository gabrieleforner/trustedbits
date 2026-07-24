namespace Trustedbits.ApiServer.Services.Schemas;

public abstract record BaseScopeSchema
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Value { get; init; }
    public required string Description { get; init; }
}

public record CreateScopeResponse : BaseScopeSchema {}
public record DescribeScopeResponse : BaseScopeSchema {}