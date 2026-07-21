namespace Trustedbits.ApiServer.Services.Schemas;

public record CreateScopeResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Value { get; init; }
    public required string Description { get; init; }
}