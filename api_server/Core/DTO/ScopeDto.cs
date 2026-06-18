namespace Trustedbits.ApiServer.Core.Dto;

/// <summary>
/// DTO for describing a RBAC scope.
/// </summary>
public class ScopeDto
{
    /// <summary>
    /// Unique identifier of the scope.
    /// </summary>
    public Guid? ScopeId { get; set; } = Guid.Empty;
    /// <summary>
    /// Name of the scope.
    /// </summary>
    public string? ScopeName { get; set; } = string.Empty;
    /// <summary>
    /// Value of the scope.
    /// </summary>
    public string? ScopeValue { get; set; } = string.Empty;
    /// <summary>
    /// Description of the scope.
    /// </summary>
    public string? ScopeDescription { get; set; } = string.Empty;
}