namespace Trustedbits.ApiServer.Models.Entities;

/// <summary>
/// Entity describing a scope
/// </summary>
public class Scope
{
    /// <summary>
    /// UUID of the scope, regardless of the tenant
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// UUID of the tenant it belongs to
    /// </summary>
    public Guid ParentTenantId { get; set; }
    
    /// <summary>
    /// Navigation property to the parent tenant entity
    /// </summary>
    public Tenant ParentTenant { get; set; }
    
    /// <summary>
    /// Name of the scope (e.g. "MetricsWrite")
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Actual scope string (e.g. "metrics:write")
    /// </summary>
    public string Value { get; set; }
    
    /// <summary>
    /// Indicates wheter the scope is active or not
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Navigation property to a list of roles that provide this scope
    /// </summary>
    public List<RoleScope<Guid>> RoleScopes { get; set; }
    
    /// <summary>
    /// Description of what this scope grants
    /// </summary>
    public string? Description { get; set; }
}