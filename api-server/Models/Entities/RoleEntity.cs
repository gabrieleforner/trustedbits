using Microsoft.AspNetCore.Identity;
using TrustedbitsApiServer.Models.Entities;

namespace TrustedbitsApiServer.Models;

/// <summary>
/// Define a role that exists within a tenant
/// </summary>
public class Role : IdentityRole<Guid>, ITenantAssociated
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
}

public class RoleUserBinding : IdentityUserRole<Guid>, ITenantAssociated
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
}