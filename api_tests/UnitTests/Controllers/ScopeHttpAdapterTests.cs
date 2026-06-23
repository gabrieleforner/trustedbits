using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trustedbits.ApiServer.Adapters.Http;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;
using Trustedbits.ApiTests.TestSupport;

namespace Trustedbits.ApiTests.UnitTests.Controllers;

[TestClass]
public class ScopeHttpAdapterTests
{
    private Mock<IScopeService> _serviceMock = null!;
    private ScopeHttpAdapter _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _serviceMock = new Mock<IScopeService>(MockBehavior.Strict);
        _controller = new ScopeHttpAdapter(_serviceMock.Object);
    }

    [TestMethod]
    public async Task CreateAsync_ValidScope_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var dto = TestData.ScopeDto();
        _serviceMock.Setup(x => x.Create(dto)).ReturnsAsync(new Result<ScopeDto>(dto));

        // Act
        var result = await _controller.CreateAsync(dto);

        // Assert
        var created = result as CreatedAtActionResult;
        Assert.IsNotNull(created);
        Assert.AreEqual(nameof(ScopeHttpAdapter.GetById), created.ActionName);
        Assert.AreSame(dto, created.Value);
        Assert.AreEqual(TestData.ScopeId, created.RouteValues!["id"]);
    }

    [TestMethod]
    public async Task CreateAsync_BadRequestResult_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = TestData.ScopeDto(name: null);
        var error = new ErrorDto("Scope name is required");
        _serviceMock.Setup(x => x.Create(dto)).ReturnsAsync(new Result<ScopeDto>(error, ErrorType.BadRequest));

        // Act
        var result = await _controller.CreateAsync(dto);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreSame(error, badRequest.Value);
    }

    [TestMethod]
    public async Task CreateAsync_ConflictResult_ShouldReturnConflict()
    {
        // Arrange
        var dto = TestData.ScopeDto();
        var error = new ErrorDto("Conflicting value found");
        _serviceMock.Setup(x => x.Create(dto)).ReturnsAsync(new Result<ScopeDto>(error, ErrorType.Conflict));

        // Act
        var result = await _controller.CreateAsync(dto);

        // Assert
        var conflict = result as ConflictObjectResult;
        Assert.IsNotNull(conflict);
        Assert.AreSame(error, conflict.Value);
    }

    [TestMethod]
    public async Task GetById_ExistingScope_ShouldReturnOk()
    {
        // Arrange
        var dto = TestData.ScopeDto();
        _serviceMock.Setup(x => x.Get(TestData.ScopeId)).ReturnsAsync(new Result<ScopeDto>(dto));

        // Act
        var result = await _controller.GetById(TestData.ScopeId);

        // Assert
        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreSame(dto, ok.Value);
    }

    [TestMethod]
    public async Task GetById_InvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var error = new ErrorDto("Scope ID is not valid");
        _serviceMock.Setup(x => x.Get(Guid.Empty)).ReturnsAsync(new Result<ScopeDto>(error, ErrorType.BadRequest));

        // Act
        var result = await _controller.GetById(Guid.Empty);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);
    }

    [TestMethod]
    public async Task GetById_MissingScope_ShouldReturnNotFound()
    {
        // Arrange
        var error = new ErrorDto("Scope not found");
        _serviceMock.Setup(x => x.Get(TestData.ScopeId)).ReturnsAsync(new Result<ScopeDto>(error, ErrorType.NotFound));

        // Act
        var result = await _controller.GetById(TestData.ScopeId);

        // Assert
        Assert.IsInstanceOfType<NotFoundObjectResult>(result);
    }

    [TestMethod]
    public async Task GetAll_ValidPaging_ShouldReturnOk()
    {
        // Arrange
        var data = new[] { TestData.ScopeDto() }.ToAsyncEnumerable();
        _serviceMock.Setup(x => x.Get(1, 10)).ReturnsAsync(new Result<IAsyncEnumerable<ScopeDto>>(data));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreSame(data, ok.Value);
    }

    [TestMethod]
    public async Task GetAll_InvalidPaging_ShouldReturnBadRequest()
    {
        // Arrange
        var error = new ErrorDto("Invalid page number");
        _serviceMock.Setup(x => x.Get(0, 10)).ReturnsAsync(new Result<IAsyncEnumerable<ScopeDto>>(error, ErrorType.BadRequest));

        // Act
        var result = await _controller.GetAll(0, 10);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreSame(error, badRequest.Value);
    }

    [TestMethod]
    public async Task UpdateAsync_ValidScope_ShouldReturnOk()
    {
        // Arrange
        var dto = TestData.ScopeDto();
        _serviceMock.Setup(x => x.Update(TestData.ScopeId, dto)).ReturnsAsync(new Result<ScopeDto>(dto));

        // Act
        var result = await _controller.UpdateAsync(TestData.ScopeId, dto);

        // Assert
        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreSame(dto, ok.Value);
    }

    [DataTestMethod]
    [DataRow(ErrorType.BadRequest, typeof(BadRequestObjectResult))]
    [DataRow(ErrorType.NotFound, typeof(NotFoundObjectResult))]
    [DataRow(ErrorType.Conflict, typeof(ConflictObjectResult))]
    public async Task UpdateAsync_FailedResult_ShouldReturnExpectedHttpResult(ErrorType errorType, Type expectedType)
    {
        // Arrange
        var dto = TestData.ScopeDto();
        _serviceMock.Setup(x => x.Update(TestData.ScopeId, dto))
            .ReturnsAsync(new Result<ScopeDto>(new ErrorDto("failed"), errorType));

        // Act
        var result = await _controller.UpdateAsync(TestData.ScopeId, dto);

        // Assert
        Assert.IsInstanceOfType(result, expectedType);
    }

    [TestMethod]
    public async Task DeleteAsync_ExistingScope_ShouldReturnOkTrue()
    {
        // Arrange
        _serviceMock.Setup(x => x.Delete(TestData.ScopeId)).ReturnsAsync(new Result<bool>(true));

        // Act
        var result = await _controller.DeleteAsync(TestData.ScopeId);

        // Assert
        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [DataTestMethod]
    [DataRow(ErrorType.BadRequest, typeof(BadRequestObjectResult))]
    [DataRow(ErrorType.NotFound, typeof(NotFoundObjectResult))]
    public async Task DeleteAsync_FailedResult_ShouldReturnExpectedHttpResult(ErrorType errorType, Type expectedType)
    {
        // Arrange
        _serviceMock.Setup(x => x.Delete(TestData.ScopeId))
            .ReturnsAsync(new Result<bool>(new ErrorDto("failed"), errorType));

        // Act
        var result = await _controller.DeleteAsync(TestData.ScopeId);

        // Assert
        Assert.IsInstanceOfType(result, expectedType);
    }
}
