using Microsoft.Extensions.Logging;
using Moq;

namespace Trustedbits.ApiTests.TestSupport;

internal static class MockExtensions
{
    public static void VerifyLogInformation<T>(this Mock<ILogger<T>> logger, Times times)
    {
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}
