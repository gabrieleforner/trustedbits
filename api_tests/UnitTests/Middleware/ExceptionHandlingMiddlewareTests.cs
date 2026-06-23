using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustedbits.ApiServer.Adapters.Http.Middleware;

namespace Trustedbits.ApiTests.UnitTests.Middleware;

[TestClass]
public class ExceptionHandlingMiddlewareTests
{
    [TestMethod]
    public async Task TryHandleAsync_UnhandledException_ShouldWriteJsonInternalServerError()
    {
        // Arrange
        var middleware = new ExceptionHandlingMiddleware();
        var context = new DefaultHttpContext();
        await using var body = new MemoryStream();
        context.Response.Body = body;

        // Act
        var handled = await middleware.TryHandleAsync(context, new InvalidOperationException("boom"), CancellationToken.None);

        // Assert
        Assert.IsTrue(handled);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        StringAssert.StartsWith(context.Response.ContentType, "application/json");
        body.Position = 0;
        var payload = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(body);
        Assert.AreEqual("An unexpected error occurred.", payload!["message"]);
    }
}
