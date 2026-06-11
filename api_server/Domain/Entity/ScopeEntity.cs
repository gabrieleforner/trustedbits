namespace Trustedbits.ApiServer.Domain.Entity;

public class ScopeEntity
{
    public Guid Id { get; set; } = Guid.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}