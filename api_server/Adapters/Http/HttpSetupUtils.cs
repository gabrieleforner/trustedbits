using Trustedbits.ApiServer.Adapters.Http.Middleware;

namespace Trustedbits.ApiServer.Adapters.Http;

/// <summary>
/// Helper methods to register HTTP-related services and configure the request pipeline.
/// Used by application startup to centralize controller registration, exception handling and routing.
/// </summary>
public static class HttpSetupUtils
{
    /// <summary>
    /// Registers MVC controllers and the global exception handler middleware into the DI container.
    /// </summary>
    /// <param name="builder">The web application builder used to register services.</param>
    public static void SetupMiddlewares(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
    }
    
    /// <summary>
    /// Configures the HTTP request processing pipeline (middleware) for the application.
    /// </summary>
    /// <param name="app">The built web application instance.</param>
    public static void SetupPipeline(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.MapControllers();
    }
}