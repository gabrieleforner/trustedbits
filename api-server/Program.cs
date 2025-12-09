using api_server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StackExchange.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Setup persistent DB layer
builder.Services.AddDbContext<ServiceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["SqlServerConnectionString"]);
    options.LogTo(Console.WriteLine, new []
    {
        RelationalEventId.ConnectionCreated,
        RelationalEventId.ConnectionOpened,
        RelationalEventId.ConnectionClosed
    }, LogLevel.Information);
    options.LogTo(Console.WriteLine, new []{ RelationalEventId.ConnectionError }, LogLevel.Critical);
});
builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));

// Setup session cache DB layer
builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
{
    var muxConnectionString = builder.Configuration["RedisConnectionString"];
    if(muxConnectionString == null)
        throw new Exception("Missing connection string for Redis!");
    
    var muxConnection = ConnectionMultiplexer.Connect(muxConnectionString);
    muxConnection.ConnectionFailed += (sender, args) =>
    {
        Console.WriteLine($"Failed to connect to Redis server. Error Type: {args.FailureType}");
        Console.WriteLine(args.Exception?.Message);
    };
    return muxConnection;
});
builder.Services.AddScoped(typeof(CacheController<>), typeof(CacheController<>));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.Services.GetRequiredService<IConnectionMultiplexer>();  // Force initialisation of Redis connection

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    if (dbContext.Database.CanConnect())
    {
        logger.LogInformation("API server connected to SQL server");
    }
    else
    {
        logger.LogError("Failed to connect to SQL server");
        Environment.Exit(-1);
    }
    dbContext.Database.EnsureCreated();
}

app.MapControllers();
app.Run();