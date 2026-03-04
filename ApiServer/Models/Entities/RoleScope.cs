namespace Trustedbits.ApiServer.Models.Entities;

public class RoleScope<TKey>
{
    public virtual TKey ScopeId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the primary key of the role that is linked to the user.
    /// </summary>
    public virtual TKey RoleId { get; set; } = default!;
}