using Microsoft.AspNetCore.Identity;
using TrustedbitsApiServer.Models.Entities;

namespace TrustedbitsApiServer.Models;

/// <summary>
/// Extends a <see cref="IdentityUser"/> for support tenancy.
/// </summary>
public class User : IdentityUser<Guid>, ITenantAssociated
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
}