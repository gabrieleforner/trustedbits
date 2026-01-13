using ApiServer.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Services;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantSettings> Settings { get; set; }
    public DbSet<Scope> Scopes { get; set; }
    public DbSet<ScopeAction> ScopeActions { get; set; }
    
    public AppDbContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Tenant>(tenant =>
        {
            tenant.ToTable("TrustedbitsTenants");
            
            tenant.HasKey(x => x.Id);
            tenant.HasIndex(x => x.Name).IsUnique();

            // Setup Tenant -> settings (one-to-one)
            tenant.HasOne(t => t.Settings)
                .WithOne(ts => ts.ParentTenant);
            
            // Setup Tenant -> users (one-to-many)
            tenant.HasMany<User>(t => t.Users)
                .WithOne(u => u.ParentTenant)
                .HasForeignKey(u => u.ParentTenantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Setup Tenant -> roles (one-to-many)
            tenant.HasMany<Role>(t => t.Roles)
                .WithOne(u => u.ParentTenant)
                .HasForeignKey(u => u.ParentTenantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Setup Tenant -> scopes (one-to-many)
            tenant.HasMany<Scope>(t => t.Scopes)
                .WithOne(u => u.ParentTenant)
                .HasForeignKey(u => u.ParentTenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TenantSettings>(settings =>
        {
            settings.ToTable("TrustedbitsTenantSettings");
            settings.HasKey(ts => ts.ParentTenantId);
        });
        modelBuilder.Entity<IdentityUserRole<Guid>>(userRole =>
        {
            userRole.ToTable("TrustedbitsUserRoles");
            userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
            
            userRole.HasOne<User>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            userRole.HasOne<Role>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.NoAction);
        });
        
        modelBuilder.Entity<Role>().ToTable("TrustedbitsRoles");
        modelBuilder.Entity<User>().ToTable("TrustedbitsUsers");
        
        modelBuilder.Entity<Scope>(scope =>
        {
            scope.ToTable("TrustedbitsScopes");
            
            scope.HasKey(s => s.ParentTenantId);
            scope.HasMany<ScopeAction>()
                .WithOne(sa => sa.ParentScope)
                .HasForeignKey(sa => sa.ParentScopeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ScopeAction>(scope =>
        {
            scope.ToTable("TrustedbitsScopeActions");
            
            scope.HasKey(s => s.ParentScopeId);
        });
    }
}