using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Adapters.Http;
using Trustedbits.ApiServer.Core;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Domain.Repository;
using Trustedbits.ApiServer.Infrastructure.AutoMapperProfiles;
using Trustedbits.ApiServer.Infrastructure.EFCore;

namespace Trustedbits.ApiServer;

/// <summary>
/// Application entry point and startup configuration for the API server.
/// Contains methods used to configure services, AutoMapper profiles, DI bindings
/// and to initialize the database on startup.
/// </summary>
public class Program
{
    /// <summary>
    /// Application entry point. Configures the host, services, middlewares and runs the web application.
    /// Reads the <c>DB_CONNECTION_STRING</c> environment variable to configure the DB provider.
    /// </summary>
    /// <param name="args">Command-line arguments forwarded to the host builder.</param>
    public static void Main(string[] args)
    {
        var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContextFactory<ServerDbContext>(opt =>
        {
            opt.UseSqlServer(dbConnectionString);
        }, ServiceLifetime.Scoped);

        SetupDIBindings(builder);
        SetupMapper(builder);

        HttpSetupUtils.SetupMiddlewares(builder);
        
        var app = builder.Build();

        SetupDatabase(app);
        HttpSetupUtils.SetupPipeline(app);
        
        app.Run();
    }

    /// <summary>
    /// Registers AutoMapper profiles used by the application.
    /// </summary>
    /// <param name="builder">The web application builder used to register services.</param>
    private static void SetupMapper(WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ScopeMapProfile>();
            cfg.AddProfile<RoleMapProfile>();
        });
    }

    /// <summary>
    /// Registers dependency-injection bindings for repositories and services.
    /// </summary>
    /// <param name="builder">The web application builder used to register services.</param>
    private static void SetupDIBindings(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericGenericRepositoryEfCoreImpl<>));
        builder.Services.AddScoped<IScopeRepository, ScopeRepositoryImpl>();
        builder.Services.AddScoped<IRoleRepository, RoleRepositoryImpl>();
        builder.Services.AddScoped<IScopeService, ScopeService>();
    }
    
    /// <summary>
    /// Ensures the database schema is created and up-to-date by running EF Core migrations.
    /// This method creates a scope to resolve the <see cref="ServerDbContext"/>.
    /// </summary>
    /// <param name="app">The built web application instance.</param>
    private static void SetupDatabase(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
            db.Database.Migrate();
        }
    }
}