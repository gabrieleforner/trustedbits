using Microsoft.AspNetCore.Identity;

namespace ApiServer.Models.Entities;

public class Role : IdentityRole<Guid>
{
    public Guid ParentTenantId { get; set; }
    public Tenant ParentTenant { get; set; }
}