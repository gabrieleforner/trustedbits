namespace Trustedbits.ApiServer.Domain.Entity;


/// <summary>
/// Implementation-agnostic representation of an
/// RBAC scope.
/// </summary>
public class ScopeEntity
{
    /// <summary>
    /// Unique identifier of the scope.
    /// </summary>
    public Guid Id { get; set; } = Guid.Empty;
    /// <summary>
    /// Name of the scope used when representing it outside the domain.
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;
    /// <summary>
    /// Normalized (full lowercase) name of the scope.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>
    /// Value of the scope (resource and verb separated by a colon)
    /// </summary>
    public string Value { get; set; } = string.Empty;
    /// <summary>
    /// Description of the scope.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to all roles owning a scope
    /// </summary>
    public ICollection<RoleEntity> RoleEntities = new List<RoleEntity>();
}