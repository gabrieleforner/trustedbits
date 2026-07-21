using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Infrastructure.EntityFramework;

public class ServerDbContext : DbContext
{
    public DbSet<Scope>  Scopes { get; set; }

    public ServerDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureScopeEntity(modelBuilder);
    }

    private static void ConfigureScopeEntity(ModelBuilder modelBuilder)
    {
        // Scope Indexes
        modelBuilder.Entity<Scope>()
            .HasIndex(s => s.NormalizedName)
            .IsUnique();
        modelBuilder.Entity<Scope>()
            .HasIndex(s => s.Value)
            .IsUnique();
        
        
        // Scope Properties
        modelBuilder.Entity<Scope>()
            .Property(s => s.Name)
            .HasMaxLength(Scope.MaxNameLength)
            .IsRequired();
        modelBuilder.Entity<Scope>()
            .Property(s => s.NormalizedName)
            .HasMaxLength(Scope.MaxNameLength)
            .IsRequired();
        
        modelBuilder.Entity<Scope>()
            .Property(s => s.Value)
            .HasMaxLength(Scope.MaxValueLength)
            .IsRequired();

        modelBuilder.Entity<Scope>()
            .Property(s => s.Description)
            .HasMaxLength(Scope.MaxDescriptionLength);
    }
}