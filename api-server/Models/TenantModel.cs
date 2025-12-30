namespace TrustedbitsApiServer.Models;

/// <summary>
/// Defines what an element that is bound to a tenant must have
/// </summary>
public interface ITenantAssociated
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
}

/// <summary>
/// Defines a single tenant
/// </summary>
public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    // Navigation Properties
    public List<Application> TenantApplications { get; set; }
    public List<User> TenantUsers { get; set; }
    public List<Role> TenantRoles { get; set; }
}