namespace Trustedbits.ApiServer.Core.DTO;

public class ScopeDto
{
    public Guid? ScopeId { get; set; } = Guid.Empty;
    public string? ScopeName { get; set; } = string.Empty;
    public string? ScopeValue { get; set; } = string.Empty;
    public string? ScopeDescription { get; set; } = string.Empty;
}