using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Infrastructure.EFCore;

public class ServerDbContext : DbContext
{
    public DbSet<ScopeEntity> Scopes { get; set; }

    public ServerDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SetupScopeEntity(modelBuilder);
    }

    static void SetupScopeEntity(ModelBuilder modelBuilder)
    {
        // Configure indexes
        // Scope Entity
        modelBuilder.Entity<ScopeEntity>()
            .HasIndex(s => s.Id)
            .IsUnique();
        modelBuilder.Entity<ScopeEntity>()
            .HasIndex(s => s.NormalizedName)
            .IsUnique();
        modelBuilder.Entity<ScopeEntity>()
            .HasIndex(s => s.Value)
            .IsUnique();

        // Role Entity
        modelBuilder.Entity<RoleEntity>()
            .HasIndex(s => s.Id)
            .IsUnique();
        modelBuilder.Entity<RoleEntity>()
            .HasIndex(s => s.NormalizedName)
            .IsUnique();
        
        // Configure properties (max length)
        // Scope Entity
        modelBuilder.Entity<ScopeEntity>()
            .Property(s => s.NormalizedName)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<ScopeEntity>()
            .Property(s => s.DisplayName)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<ScopeEntity>()
            .Property(s => s.Value)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<ScopeEntity>()
            .Property(s => s.Description)
            .HasMaxLength(255);
        
        // Role Entity
        modelBuilder.Entity<RoleEntity>()
            .Property(r => r.NormalizedName)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<RoleEntity>()
            .Property(r => r.DisplayName)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<RoleEntity>()
            .Property(r => r.Description)
            .HasMaxLength(255);
    }
}