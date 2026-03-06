using Trustedbits.ApiServer.Data;
using Trustedbits.ApiServer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Data.AutomapperProfiles;
using Trustedbits.ApiServer.Data.Repository;

namespace Trustedbits.ApiServer;


/// <summary>
/// Entrypoint class of the application
/// </summary>
public class Program
{
    
    /// <summary>
    /// Entrypoint method of the application
    /// </summary>
    public static void Main(string[] args)
    {
        // Create application and  set up Dependency Injection
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        
        // Setup Entity Framework and Identity Framework
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration["SQL_CONNECTION_STRING"]);
            if (builder.Configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            {
                options
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        });
        builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<AppDbContext>();
        
        
        // Setup DI
        builder.Services.AddScoped(typeof(IRepository<>), typeof(EFCoreRepository<>));
        
        // Setup AutoMapper profiles
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ScopeMappings>();
            cfg.AddProfile<RoleMappings>();
            cfg.AddProfile<TenantMappings>();
        });
        
        var app = builder.Build();

        
        // Seed database tables
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dbService = services.GetRequiredService<AppDbContext>().Database;

            if (app.Environment.IsDevelopment())
            {
                dbService.EnsureDeleted();
                dbService.EnsureCreated();
            }
            else dbService.Migrate();
        }
        
        app.MapControllers();
        app.Run();
    }
}