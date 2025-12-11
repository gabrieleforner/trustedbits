using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace api_server.Services;

public static class ServicesBuilder
{
    private static void SetupCacheDI(WebApplicationBuilder builder)
    {
        // Setup session cache DB layer
        builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var muxConnectionString = builder.Configuration["RedisConnectionString"];
            if (muxConnectionString == null)
                throw new Exception("Missing connection string for Redis!");

            // Setup Redis event management
            var muxConnection = ConnectionMultiplexer.Connect(muxConnectionString);
            muxConnection.ConnectionFailed += RedisEventController.OnRedisConnectionFailed;
            muxConnection.ConnectionRestored += RedisEventController.OnRedisConnectionFailed;
            return muxConnection;
        });
        builder.Services.AddScoped(typeof(CacheController<>), typeof(CacheController<>));
    }
    private static void SetupRepositoriesDI(WebApplicationBuilder builder)
    {
        // Setup persistent DB layer
        builder.Services.AddSingleton<SQLEventController>();
        builder.Services.AddDbContext<ServiceDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(builder.Configuration["SqlServerConnectionString"], optionsBuilder =>
            {
                // Retry 5 times with a 10 seconds interval
                optionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });

            var sqlEventController =
                new SQLEventController(serviceProvider.GetRequiredService<ILogger<SQLEventController>>());
            options.AddInterceptors(sqlEventController); // Enable SQL custom event logging
        });
        builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));

    }
    public static void SetupServices(WebApplicationBuilder builder)
    {
        // Setup of ASP.NET dep injection for Redis/EntityFramework
        SetupCacheDI(builder);
        SetupRepositoriesDI(builder);
        
        // Setup health checks
        builder.Services.AddHealthChecks()
            .AddSqlServer(
                connectionString: builder.Configuration["SqlServerConnectionString"],
                name: "sql",
                tags: ["persistent-db"],
                failureStatus: HealthStatus.Unhealthy)
            .AddRedis(
                redisConnectionString: builder.Configuration["RedisConnectionString"],
                tags: ["cache-db"],
                failureStatus: HealthStatus.Unhealthy);
    }
}