using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;

namespace Trustedbits.ApiTests.UnitTests.Results;

[TestClass]
public class ResultTests
{
    [TestMethod]
    public void Constructor_WithData_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var dto = new ScopeDto { ScopeName = "Read Users" };

        // Act
        var result = new Result<ScopeDto>(dto);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreSame(dto, result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public void Constructor_WithError_ShouldCreateFailedResult()
    {
        // Arrange
        var error = new ErrorDto("Invalid");

        // Act
        var result = new Result<ScopeDto>(error, ErrorType.BadRequest);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreSame(error, result.Error);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
        Assert.IsNull(result.Data);
    }
}
