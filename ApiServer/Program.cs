using Trustedbits.ApiServer.Data;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Trustedbits.ApiServer;

/// <summary>
/// Entrypoint class of the API server
/// </summary>
public class Program
{
    /// <summary>
    /// Entrypoint of the program
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
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
        builder.Services.AddScoped(typeof(IRepository<>), typeof(DbRepositoryImpl<>));
        
        var app = builder.Build();

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