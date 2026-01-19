using Microsoft.AspNetCore.Identity;

namespace Trustedbits.ApiServer.Models.Entities;

public class Role : IdentityRole<Guid>
{
    public Guid ParentTenantId { get; set; }
    public Tenant ParentTenant { get; set; }

    public List<RoleScope<Guid>> RoleScopes { get; set; }
}