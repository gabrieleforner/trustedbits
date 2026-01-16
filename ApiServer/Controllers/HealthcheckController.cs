using ApiServer.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace ApiServer.Controllers;

public class ApplicationHealthcheckController : IHealthCheck
{
    private readonly ILogger<ApplicationHealthcheckController> _logger;
    private readonly AppDbContext _dbContext;
    private readonly IConnectionMultiplexer _distributedCache;

    public ApplicationHealthcheckController(ILogger<ApplicationHealthcheckController> logger, AppDbContext dbContext,
        IConnectionMultiplexer distributedCache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _distributedCache = distributedCache;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Healthcheck request received!");

        var dbCanConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
        var cacheCanConnect = _distributedCache.IsConnected;
        
        if (!dbCanConnect)
        {
            HealthCheckResult.Unhealthy("Redis not connected.");
        }
        if (!dbCanConnect)
        {
            HealthCheckResult.Unhealthy("SQL database not connected.");
        }
        return HealthCheckResult.Healthy("All dependencies are connected!");
    } 
}