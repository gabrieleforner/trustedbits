using api_server.Entities;
using Microsoft.EntityFrameworkCore;

namespace api_server.Services;

public class ServiceDbContext : DbContext
{
    public readonly DbSet<User> UsersSet;

    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}