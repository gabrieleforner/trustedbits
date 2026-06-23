using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustedbits.ApiServer.Core;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;
using Trustedbits.ApiTests.TestSupport;

namespace Trustedbits.ApiTests.UnitTests.Helpers;

[TestClass]
public class ResultHelpersTests
{
    [TestMethod]
    public void NotFoundError_MissingId_ShouldReturnNotFoundResult()
    {
        // Arrange / Act
        var result = ResultHelpers<ScopeDto>.NotFoundError(TestData.ScopeId);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.NotFound, result.ErrorType);
        Assert.AreEqual("Resource not found", result.Error!.Message);
        Assert.IsNotNull(result.Error.Detail);
    }

    [TestMethod]
    public void ConflictError_DuplicateAttribute_ShouldReturnConflictResult()
    {
        // Arrange / Act
        var result = ResultHelpers<ScopeDto>.ConflictError("ScopeName", "Read Users");

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.Conflict, result.ErrorType);
        Assert.AreEqual("Conflicting value found", result.Error!.Message);
    }

    [TestMethod]
    public void InvalidScopeIdError_EmptyId_ShouldReturnBadRequestResult()
    {
        // Arrange / Act
        var result = ResultHelpers<ScopeDto>.InvalidIdError();

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
        Assert.AreEqual("Provided ID is not valid", result.Error!.Message);
    }

    [DataTestMethod]
    [DataRow(0, 10, "Invalid page number")]
    [DataRow(1, 0, "Invalid page size")]
    public void ValidatePagingSettings_InvalidPaging_ShouldReturnBadRequest(int page, int pageSize, string expectedMessage)
    {
        // Arrange / Act
        var result = ResultHelpers<ScopeDto>.ValidatePagingSettings(page, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
        Assert.AreEqual(expectedMessage, result.Error!.Message);
    }

    [TestMethod]
    public void ValidatePagingSettings_ValidPaging_ShouldReturnNull()
    {
        // Arrange / Act
        var result = ResultHelpers<ScopeDto>.ValidatePagingSettings(1, 10);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void BadRequest_CustomDetail_ShouldReturnBadRequestResult()
    {
        // Arrange
        var detail = new { Field = "ScopeName" };

        // Act
        var result = ResultHelpers<ScopeDto>.BadRequest("Invalid", detail);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
        Assert.AreSame(detail, result.Error!.Detail);
    }
}
