using Microsoft.AspNetCore.Identity;
using TrustedbitsApiServer.Models;
using TrustedbitsApiServer.Models.Entities;

namespace TrustedbitsApiServer.Services;

public class InstanceSetup
{
    private ILogger _logger;
    private ServerDbContext _dbContext;
    private UserManager<User> _userManager;
    private RoleManager<Role> _roleManager;
    
    private Guid _defaultTenantId;
    
    public static string DefaultTenantName = "TrustedbitsAdmin";
    public static string DefaultAdminRoleName = "PlatformAdmin";
    public static string DefaultUserName = "tadbmin";
    public static string DefaultEmail = "admin@trustedbits.local";
    public static string DefaultPassword = "TrustedB1ts@Adm1N";

    public InstanceSetup(
        ILogger logger,
        ServerDbContext dbContext,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _logger = logger;
        if (dbContext == null)
        {
            throw new ArgumentNullException($"ServerDbContext instance is null!");
        }
        _dbContext = dbContext;

        if (userManager == null)
        {
            throw new ArgumentNullException($"UserManager<User> is null!");
        }
        _userManager = userManager;

        if (roleManager == null)
        {
            throw new ArgumentNullException($"RoleManager<Role> is null!");
        }
        _roleManager = roleManager;
    }
    
    public void SetupDefaultTenant()
    {
        _logger.LogInformation("Looking up default tenant...");

        var tenant = _dbContext.Tenants
            .FirstOrDefault(t => t.Name == DefaultTenantName);

        if (tenant == null)
        {
            _logger.LogInformation("No default tenant. Creating default tenant...");

            tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = DefaultTenantName
            };

            _dbContext.Tenants.Add(tenant);
            _dbContext.SaveChanges();
        }
        else
        {
            _logger.LogInformation("Default tenant already exists");
        }
        _defaultTenantId = tenant.Id;

        _logger.LogInformation("Looking up default admin role...");

        var containDefaultRole = _roleManager.Roles
            .Any(r => r.Name == DefaultAdminRoleName && r.TenantId == _defaultTenantId);

        if (!containDefaultRole)
        {
            _logger.LogInformation("Setting up default admin role...");

            _roleManager
                .CreateAsync(new Role
                {
                    Name = DefaultAdminRoleName,
                    TenantId = _defaultTenantId
                })
                .GetAwaiter()
                .GetResult();
        }
    }

    public void SetupDefaultAccount()
    {
        var matchingUser = _userManager
            .Users
            .FirstOrDefault(u => u.UserName == DefaultUserName &&
                                 u.TenantId == _defaultTenantId);

        if (matchingUser != null)
        {
            _logger.LogInformation("Default account already exists");
            return;
        }

        _logger.LogInformation("Creating default account...");

        var result = _userManager
            .CreateAsync(new User
            {
                UserName = DefaultUserName,
                Email = DefaultEmail,
                EmailConfirmed = true,
                TenantId = _defaultTenantId
            }, DefaultPassword)
            .GetAwaiter()
            .GetResult();
        
        _logger.LogInformation("Account created");
        _logger.LogInformation($"Default account EMAIL: {DefaultEmail}");
        _logger.LogInformation($"Default account USERNAME: {DefaultUserName}");
        _logger.LogInformation($"Default account PASSWORD: {DefaultPassword}");

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create default account: {errors}");
        }
    }
}
