using Microsoft.AspNetCore.Diagnostics;

namespace Trustedbits.ApiServer.Adapters.Http.Middleware;

/// <summary>
/// Global exception handler that serializes an internal server error response for unhandled exceptions.
/// Registered via <c>AddExceptionHandler{T}()</c> in startup and called by the framework when exceptions bubble up.
/// </summary>
public class ExceptionHandlingMiddleware : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle the provided exception by writing a 500 JSON response.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="cancellationToken">Cancellation token used when writing the response.</param>
    /// <returns>True when the exception was handled and no further processing is required.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var errorResponse = new { Message = "An unexpected error occurred." };
        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
        return true;
    }
}