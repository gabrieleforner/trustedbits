using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Infrastructure;
using Trustedbits.ApiServer.Infrastructure.EntityFramework;

namespace Trustedbits.ApiServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        EnvLoader.Load(builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddDbContext<ServerDbContext>(options =>
        {
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString"))
                .UseExceptionProcessor();
        });
        
        var app = builder.Build();
        
        // Migrate DB schemas
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
        await context.Database.MigrateAsync();
        
        app.UseHttpsRedirection();
        await app.RunAsync();
    }
}