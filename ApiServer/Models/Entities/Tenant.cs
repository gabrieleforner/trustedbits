namespace ApiServer.Models.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public TenantSettings Settings { get; set; } =  new TenantSettings();

    public List<User> Users { get; set; } = new List<User>();
    public List<Role> Roles { get; set; } = new List<Role>();

    public List<Scope> Scopes { get; set; } = new List<Scope>();
}