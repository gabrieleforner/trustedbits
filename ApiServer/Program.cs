using Trustedbits.ApiServer.Data;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Trustedbits.ApiServer.Delegates;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Services;

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
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Check connection strings presence
            if(builder.Configuration["SQL_CONNECTION_STRING"].IsNullOrEmpty())
                throw new Exception("SQL_CONNECTION_STRING is either empty or not set! Set it to a non-empty value" +
                                    " in order to fix this error.");
            
            if(builder.Configuration["REDIS_CONNECTION_STRING"].IsNullOrEmpty())
                throw new Exception("REDIS_CONNECTION_STRING is either empty or not set! Set it to a non-empty value" +
                                    " in order to fix this error.");
            
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
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["REDIS_CONNECTION_STRING"];
            });
            builder.Services.AddScoped(typeof(IRepository<>), typeof(DbRepositoryImpl<>));

            // Bind provider implementations to interfaces
            builder.Services.AddScoped<IAuthenticationDelegate, DefaultAuthenticationDelegate>();
            builder.Services.AddScoped<ISessionDelegate, DefaultSessionDelegate>();

            // Bind service interface to implementations
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            
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
        catch (Exception e)
        {
            Console.WriteLine("Trustedbits failed to start!");
            Console.WriteLine($"Reason: {e.Message}");
        }
    }
}