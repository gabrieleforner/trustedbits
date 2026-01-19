using Trustedbits.ApiServer.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Trustedbits.ApiServer.Data;

/// <inheritdoc/>
public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    /// <summary>
    /// Mapping to the <c>Tenants</c> table.
    /// </summary>
    public DbSet<Tenant> Tenants { get; set; }
    /// <summary>
    /// Mapping to the <c>TenantSettings</c> table.
    /// </summary>
    public DbSet<TenantSettings> Settings { get; set; }
    /// <summary>
    /// Mapping to the <c>Scopes</c> table.
    /// </summary>
    public DbSet<Scope> Scopes { get; set; }
    /// <summary>
    /// Mapping to the <c>ScopeActions</c> table.
    /// </summary>
    public DbSet<ScopeAction> ScopeActions { get; set; }
    
    public DbSet<RoleScope<Guid>> JoinRoleScope { get; set; }
    
    /// <inheritdoc/>
    public AppDbContext(DbContextOptions options) : base(options) { }
    
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Table Names
        // 
        modelBuilder.Entity<Tenant>().ToTable("TrustedbitsTenants");
        modelBuilder.Entity<TenantSettings>().ToTable("TrustedbitsTenantSettings");
        modelBuilder.Entity<User>().ToTable("TrustedbitsUsers");
        modelBuilder.Entity<Role>().ToTable("TrustedbitsRoles");
        modelBuilder.Entity<Scope>().ToTable("TrustedbitsScopes");
        modelBuilder.Entity<ScopeAction>().ToTable("TrustedbitsScopes");
        
        // Join tables
        modelBuilder.Entity<RoleScope<Guid>>().ToTable("TrustedbitsRoleScopes");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("TrustedbitsUserRoles");
        
        SetupTenantRelationships(modelBuilder);
        SetupScopeRelationships(modelBuilder);
        
        SetupRoleScopeJoinTable(modelBuilder);
        SetupUserRoleJoinTable(modelBuilder);
    }

    private void SetupTenantRelationships(ModelBuilder modelBuilder)
    {
        // Users -> tenant relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.ParentTenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.ParentTenantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Roles -> tenant relationship
        modelBuilder.Entity<Role>()
            .HasOne(r => r.ParentTenant)
            .WithMany(t => t.Roles)
            .HasForeignKey(r => r.ParentTenantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Roles -> tenant relationship
        modelBuilder.Entity<Scope>()
            .HasOne(r => r.ParentTenant)
            .WithMany(t => t.Scopes)
            .HasForeignKey(r => r.ParentTenantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Tenant settings -> tenant relationship
        modelBuilder.Entity<TenantSettings>()
            .HasOne(s => s.ParentTenant)
            .WithOne(t => t.Settings)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TenantSettings>()
            .HasKey(ts => ts.ParentTenantId);
    }

    private void SetupScopeRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Scope>()
            .HasKey(s => s.Id);
        
        modelBuilder.Entity<Scope>()
            .HasMany(s => s.ScopeActions)
            .WithOne(a => a.ParentScope)
            .HasForeignKey(a => a.ParentScopeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Scope>()
            .HasKey(s => s.ParentTenantId);
        modelBuilder.Entity<ScopeAction>()
            .HasKey(a => a.ParentScopeId);
    }
    

    private void SetupRoleScopeJoinTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoleScope<Guid>>()
            .HasKey(rs => new { rs.ScopeId, rs.RoleId });
        
        modelBuilder.Entity<RoleScope<Guid>>()
            .HasOne<Role>()
            .WithMany(r => r.RoleScopes)
            .HasForeignKey(rs => rs.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoleScope<Guid>>()
            .HasOne<Scope>()
            .WithMany(s => s.RoleScopes)
            .HasForeignKey(rs => rs.ScopeId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private void SetupUserRoleJoinTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
        
        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .HasOne<Role>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}