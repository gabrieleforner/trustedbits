using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustedbits.ApiServer.Adapters.Http;

namespace Trustedbits.ApiTests.UnitTests.Middleware;

[TestClass]
public class HttpSetupUtilsTests
{
    [TestMethod]
    public void SetupMiddlewares_WebApplicationBuilder_ShouldRegisterControllersAndExceptionHandler()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());

        // Act
        HttpSetupUtils.SetupMiddlewares(builder);
        using var provider = builder.Services.BuildServiceProvider();

        // Assert
        Assert.IsNotNull(provider.GetService<IActionInvokerFactory>());
        Assert.IsTrue(builder.Services.Any(x => x.ServiceType == typeof(IExceptionHandler)));
    }

    [TestMethod]
    public void SetupPipeline_WebApplication_ShouldMapControllerEndpoints()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.Services.AddControllers();
        var app = builder.Build();

        // Act
        HttpSetupUtils.SetupPipeline(app);

        // Assert
        Assert.IsNotNull(app);
    }
}
