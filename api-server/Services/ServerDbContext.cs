using TrustedbitsApiServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrustedbitsApiServer.Models.Entities;

namespace TrustedbitsApiServer.Services;

public class ServerDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    public ServerDbContext(DbContextOptions<ServerDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureTenants(builder);
        ConfigureUsers(builder);
        ConfigureApplications(builder);
        ConfigureRoles(builder);
        ConfigureIdentityJoins(builder);
    }

    // ----------------------------
    // TENANT
    // ----------------------------
    private static void ConfigureTenants(ModelBuilder builder)
    {
        builder.Entity<Tenant>(tenant =>
        {
            tenant.ToTable("Tenants");
            tenant.HasKey(t => t.Id);

            tenant.HasIndex(t => t.Name).IsUnique();

            tenant.Property(t => t.Name)
                .HasMaxLength(256)
                .IsRequired();

            tenant.HasMany(t => t.TenantUsers)
                .WithOne(u => u.Tenant)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            tenant.HasMany(t => t.TenantApplications)
                .WithOne(a => a.Tenant)
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            tenant.HasMany(t => t.TenantRoles)
                .WithOne(r => r.Tenant)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // ----------------------------
    // USER
    // ----------------------------
    private static void ConfigureUsers(ModelBuilder builder)
    {
        builder.Entity<User>(user =>
        {
            user.ToTable("Users");

            user.HasIndex(u => u.TenantId);

            user.Property(u => u.TenantId)
                .IsRequired();
        });
    }

    // ----------------------------
    // APPLICATION
    // ----------------------------
    private static void ConfigureApplications(ModelBuilder builder)
    {
        builder.Entity<Application>(application =>
        {
            application.ToTable("RegisteredApplications");
            application.HasKey(a => a.ClientId);

            application.HasIndex(a => new { a.TenantId, a.Name })
                .IsUnique();

            application.Property(a => a.Name)
                .HasMaxLength(256)
                .IsRequired();

            application.Property(a => a.ClientId)
                .HasMaxLength(256)
                .IsRequired();

            application.Property(a => a.Scope)
                .HasMaxLength(512)
                .IsRequired();

            application.Property(a => a.TenantId)
                .IsRequired();
        });
    }

    // ----------------------------
    // ROLE
    // ----------------------------
    private static void ConfigureRoles(ModelBuilder builder)
    {
        builder.Entity<Role>(role =>
        {
            role.ToTable("Roles");

            role.HasIndex(r => r.TenantId);

            role.Property(r => r.TenantId)
                .IsRequired();
        });
    }

    // ----------------------------
    // IDENTITY JOIN TABLES
    // ----------------------------
    private static void ConfigureIdentityJoins(ModelBuilder builder)
    {
        // User ↔ Role (NO CASCADE)
        builder.Entity<IdentityUserRole<Guid>>()
            .ToTable("UserRoles");

        builder.Entity<IdentityUserRole<Guid>>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<IdentityUserRole<Guid>>()
            .HasOne<Role>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<IdentityUserClaim<Guid>>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<IdentityUserLogin<Guid>>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<IdentityUserToken<Guid>>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
