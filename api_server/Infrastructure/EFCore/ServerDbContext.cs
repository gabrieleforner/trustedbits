using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Infrastructure.EFCore;

/// <summary>
/// EF Core database context for the API server.
/// Contains DbSet properties and model configuration for domain entities.
/// </summary>
public class ServerDbContext : DbContext
{
    /// <summary>
    /// DB set containing <see cref="ScopeEntity"/> records.
    /// </summary>
    public DbSet<ScopeEntity> Scopes { get; set; }
    
    /// <summary>
    /// DB set containing <see cref="RoleEntity"/>
    /// </summary>
    public DbSet<RoleEntity> Roles { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="ServerDbContext"/>.
    /// </summary>
    /// <param name="options">The options used to configure the context.</param>
    public ServerDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Called by EF Core to build the model. Configures indexes and property constraints
    /// for the domain entities used by the application.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SetupScopeEntity(modelBuilder);
    }

    /// <summary>
    /// Configures entity mappings, indexes and property constraints for ScopeEntity.
    /// This method is intentionally static to emphasize it does not depend on instance state.
    /// </summary>
    /// <param name="modelBuilder">Model builder used to configure the EF model.</param>
    static void SetupScopeEntity(ModelBuilder modelBuilder)
    {
        // Configure indexes
        modelBuilder.Entity<ScopeEntity>()
            .HasIndex(s => s.Id)
            .IsUnique();
        modelBuilder.Entity<ScopeEntity>()
            .HasIndex(s => s.NormalizedName)
            .IsUnique();
        modelBuilder.Entity<ScopeEntity>()
            .HasIndex(s => s.Value)
            .IsUnique();

        
        
        // Configure properties (max length)
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
        

    }

    /// <summary>
    /// Configures entity mappings, indexes and property constraints for RoleEntity.
    /// This method is intentionally static to emphasize it does not depend on instance state.
    /// </summary>
    /// <param name="modelBuilder">Model builder used to configure the EF model.</param>
    public void SetupRoleEntity(ModelBuilder modelBuilder)
    {
        // Configure indexes
        modelBuilder.Entity<RoleEntity>()
            .HasIndex(s => s.Id)
            .IsUnique();
        modelBuilder.Entity<RoleEntity>()
            .HasIndex(s => s.NormalizedName)
            .IsUnique();
        
        // Configure properties
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