using Microsoft.AspNetCore.Identity;
using TrustedbitsApiServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrustedbitsApiServer.Models;

namespace TrustedbitsApiServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ServerDbContext>(opt => {
            opt.UseSqlServer(GetConnectionString(builder.Configuration));
        });
        builder.Services
            .AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ServerDbContext>();
        builder.Services.AddControllers();
        
        var app = builder.Build();
        
        app.UseHttpsRedirection();
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            
            if (builder.Environment.IsDevelopment())
                dbContext.Database.EnsureCreated();
            else
                dbContext.Database.Migrate();

            var instanceSetup = new InstanceSetup(logger, dbContext, userManager, roleManager);

            instanceSetup.SetupDefaultTenant();
            instanceSetup.SetupDefaultAccount();
        }
        
        app.Run();
    }

    /// <summary>
    /// Parse the environment variables (from <c>config</c>) and generate the connection string
    /// to connect to SQL Server instance.
    /// </summary>
    /// <param name="config"> <see cref="IConfiguration" /> instance </param>
    /// <returns>The connection string (with <c>TrustServerCertificate</c> option enabled).</returns>
    /// <exception cref="ArgumentException">If one of the env vars are empty or null or if the
    /// port is not a number or the port number is more than 65535 or less than 1.</exception>
    public static string GetConnectionString(IConfiguration config)
    {
        var host = config["SQL_SERVER_HOST"];
        var port = config["SQL_SERVER_PORT"];

        var sqlUser = config["SQL_USER"];
        var sqlPassword = config["SQL_PASSWORD"];
        var sqlDatabaseName = config["SQL_DB_NAME"] ?? "master";
        
        // Network Settings
        if(host.IsNullOrEmpty())
            throw new ArgumentException("SQL_SERVER_HOST is required");
        if(port.IsNullOrEmpty())
            throw new ArgumentException("SQL_SERVER_PORT is required");
        if (!int.TryParse(port, out var sqlPort))
        {
            throw new ArgumentException("SQL_SERVER_PORT should be a number");
        }

        if (sqlPort < 1 || sqlPort > 65535)
        {
            throw new ArgumentException("SQL_SERVER_PORT should be in range 1 to 65535");
        }
        
        
        // Identity Settings
        if (sqlUser.IsNullOrEmpty())
            throw new ArgumentException("SQL_USER is required");
        if (sqlPassword.IsNullOrEmpty())
            throw new ArgumentException("SQL_PASSWORD is required");
        
        return $"Server={host},{port}; " +
               $"User Id={sqlUser}; Password = {sqlPassword}; " +
               $"Database = {sqlDatabaseName}; " +
               $"TrustServerCertificate=true";
    }
}