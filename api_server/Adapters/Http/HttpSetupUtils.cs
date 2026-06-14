using Trustedbits.ApiServer.Adapters.Http.Middleware;

namespace Trustedbits.ApiServer.Adapters.Http;

public static class HttpSetupUtils
{
    public static void SetupMiddlewares(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
    }
    
    public static void SetupPipeline(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.MapControllers();
    }
}