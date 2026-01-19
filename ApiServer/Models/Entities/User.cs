using Microsoft.AspNetCore.Identity;

namespace Trustedbits.ApiServer.Models.Entities;

public class User : IdentityUser<Guid>
{
    public Tenant ParentTenant { get; set; }
    public Guid ParentTenantId { get; set; }
}