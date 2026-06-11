using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Repository;
using Trustedbits.ApiServer.Infrastructure.AutoMapperProfiles;
using Trustedbits.ApiServer.Infrastructure.EFCore;

namespace Trustedbits.ApiServer;

public class Program
{
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

        builder.Services.AddControllers();
        
        var app = builder.Build();

        SetupDatabase(app);
        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }

    private static void SetupMapper(WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ScopeMapProfile>();
        });
    }

    private static void SetupDIBindings(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericGenericRepositoryEfCoreImpl<>));
        builder.Services.AddScoped<IScopeRepository, ScopeRepositoryImpl>();
        builder.Services.AddScoped<IScopeRepository, ScopeRepositoryImpl>();
    }

    private static void SetupDatabase(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
            db.Database.Migrate();
        }
    }
}