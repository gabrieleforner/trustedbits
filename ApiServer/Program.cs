using ApiServer.Controllers;
using ApiServer.Interfaces;
using ApiServer.Models.Entities;
using ApiServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Role = ApiServer.Models.Entities.Role;

namespace ApiServer;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Verify environment variables
            if(string.IsNullOrEmpty(builder.Configuration["REDIS_CONNECTION_STRING"]))
                throw new ArgumentNullException(builder.Configuration["REDIS_CONNECTION_STRING"],
                    "Missing redis connection string! (Set the env var REDIS_CONNECTION_STRING to fix this error)");
            
            if(string.IsNullOrEmpty(builder.Configuration["SQL_CONNECTION_STRING"]))
                throw new ArgumentNullException(builder.Configuration["SQL_CONNECTION_STRING"],
                    "Missing SQL database connection string! (Set the env var SQL_CONNECTION_STRING to fix this error)");
            
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks()
                .AddCheck<ApplicationHealthcheckController>("AppDependsOn");

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
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["REDIS_CONNECTION_STRING"];
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(
                    builder.Configuration["REDIS_CONNECTION_STRING"],
                    true
                );
                return ConnectionMultiplexer.Connect(configuration);
            });


            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>();
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

            app.MapHealthChecks("/health");
            app.MapControllers();
            app.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to start up API Server!");
            Console.WriteLine($"Message: {e.Message}");
        }
    }
}