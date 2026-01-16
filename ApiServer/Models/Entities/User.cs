using Microsoft.AspNetCore.Identity;

namespace ApiServer.Models.Entities;

public class User : IdentityUser<Guid>
{
    public Tenant ParentTenant { get; set; } = new();
    public Guid ParentTenantId { get; set; }
}