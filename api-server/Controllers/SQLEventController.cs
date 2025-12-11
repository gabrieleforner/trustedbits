using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace api_server.Services;

// Interceptor for SQL Server errors
public class SQLEventController : DbConnectionInterceptor
{
    private readonly ILogger<SQLEventController> _logger;
    
    public SQLEventController(ILogger<SQLEventController> logger)
    {
        this._logger = logger;
        _logger.LogInformation("SQL Server EventController loaded");
    }
    
    public override void ConnectionFailed(DbConnection connection, ConnectionErrorEventData eventData)
    {
        _logger.LogInformation("Failed to connect to SQL Server!");
        _logger.LogInformation($"Reason: {eventData.Exception.Message}");
        Console.Write($"Originated at: {eventData.Exception.StackTrace}");
        base.ConnectionFailed(connection, eventData);
    }
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        _logger.LogInformation("Server connected to SQL Server!");
        _logger.LogInformation($"Database: {eventData.Connection.Database}");
        base.ConnectionOpened(connection, eventData);
    }
    public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData,
        CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Server connected to SQL Server!");
        _logger.LogInformation($"Database: {eventData.Connection.Database}");
        return base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
    public override void ConnectionClosed(DbConnection connection, ConnectionEndEventData eventData)
    {
        _logger.LogInformation("SQL Server connection closed!");
        _logger.LogInformation($"Database: {eventData.Connection.Database}");
        base.ConnectionClosed(connection, eventData);
    }
}