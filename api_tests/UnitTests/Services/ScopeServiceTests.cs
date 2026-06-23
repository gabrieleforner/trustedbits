using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Trustedbits.ApiServer.Core;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Domain.Repository;
using Trustedbits.ApiTests.TestSupport;

namespace Trustedbits.ApiTests.UnitTests.Services;

[TestClass]
public class ScopeServiceTests
{
    private Mock<IMapper> _mapperMock = null!;
    private Mock<IScopeRepository> _repositoryMock = null!;
    private Mock<ILogger<ScopeService>> _loggerMock = null!;
    private ScopeService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        _repositoryMock = new Mock<IScopeRepository>(MockBehavior.Strict);
        _loggerMock = new Mock<ILogger<ScopeService>>();
        _service = new ScopeService(_mapperMock.Object, _repositoryMock.Object, _loggerMock.Object);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public async Task Create_MissingName_ShouldReturnBadRequest(string? name)
    {
        // Arrange
        var dto = TestData.ScopeDto(name: name);

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
        Assert.AreEqual("Scope name is required", result.Error!.Message);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public async Task Create_MissingValue_ShouldReturnBadRequest(string? value)
    {
        // Arrange
        var dto = TestData.ScopeDto(value: value);

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
        Assert.AreEqual("Scope value is required", result.Error!.Message);
    }

    [TestMethod]
    public async Task Create_DuplicateName_ShouldReturnConflict()
    {
        // Arrange
        var dto = TestData.ScopeDto();
        _repositoryMock.Setup(x => x.GetByNameAsync(dto.ScopeName!, default))
            .ReturnsAsync(TestData.ScopeEntity());

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.Conflict, result.ErrorType);
        _repositoryMock.Verify(x => x.GetByValueAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Create_DuplicateValue_ShouldReturnConflict()
    {
        // Arrange
        var dto = TestData.ScopeDto();
        _repositoryMock.Setup(x => x.GetByNameAsync(dto.ScopeName!, default))
            .ReturnsAsync((ScopeEntity?)null);
        _repositoryMock.Setup(x => x.GetByValueAsync(dto.ScopeValue!, default))
            .ReturnsAsync(TestData.ScopeEntity());

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.Conflict, result.ErrorType);
        Assert.AreEqual("Conflicting value found", result.Error!.Message);
    }

    [TestMethod]
    public async Task Create_ValidScope_ShouldCreateAndReturnMappedDto()
    {
        // Arrange
        var dto = TestData.ScopeDto(id: Guid.NewGuid());
        var mappedEntity = TestData.ScopeEntity(id: Guid.NewGuid());
        var createdEntity = TestData.ScopeEntity(id: TestData.OtherScopeId);
        var createdDto = TestData.ScopeDto(id: TestData.OtherScopeId);
        _repositoryMock.Setup(x => x.GetByNameAsync(dto.ScopeName!, default))
            .ReturnsAsync((ScopeEntity?)null);
        _repositoryMock.Setup(x => x.GetByValueAsync(dto.ScopeValue!, default))
            .ReturnsAsync((ScopeEntity?)null);
        _mapperMock.Setup(x => x.Map<ScopeEntity>(dto)).Returns(mappedEntity);
        _repositoryMock.Setup(x => x.CreateAsync(mappedEntity, default)).ReturnsAsync(createdEntity);
        _mapperMock.Setup(x => x.Map<ScopeDto>(createdEntity)).Returns(createdDto);

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreEqual(TestData.OtherScopeId, result.Data!.ScopeId);
        Assert.AreEqual(Guid.Empty, mappedEntity.Id);
        _loggerMock.VerifyLogInformation(Times.Once());
    }

    [TestMethod]
    public async Task Create_RepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var dto = TestData.ScopeDto();
        var mappedEntity = TestData.ScopeEntity();
        _repositoryMock.Setup(x => x.GetByNameAsync(dto.ScopeName!, default)).ReturnsAsync((ScopeEntity?)null);
        _repositoryMock.Setup(x => x.GetByValueAsync(dto.ScopeValue!, default)).ReturnsAsync((ScopeEntity?)null);
        _mapperMock.Setup(x => x.Map<ScopeEntity>(dto)).Returns(mappedEntity);
        _repositoryMock.Setup(x => x.CreateAsync(mappedEntity, default)).ThrowsAsync(new InvalidOperationException("storage failed"));

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.Create(dto));
    }

    [TestMethod]
    public async Task Get_EmptyId_ShouldReturnBadRequest()
    {
        // Arrange / Act
        var result = await _service.Get(Guid.Empty);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
    }

    [TestMethod]
    public async Task Get_MissingScope_ShouldReturnNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, false, default)).ReturnsAsync((ScopeEntity?)null);

        // Act
        var result = await _service.Get(TestData.ScopeId);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.NotFound, result.ErrorType);
    }

    [TestMethod]
    public async Task Get_ExistingScope_ShouldReturnMappedDto()
    {
        // Arrange
        var entity = TestData.ScopeEntity();
        var dto = TestData.ScopeDto();
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, default)).ReturnsAsync(entity);
        _mapperMock.Setup(x => x.Map<ScopeDto>(entity)).Returns(dto);

        // Act
        var result = await _service.Get(TestData.ScopeId);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreSame(dto, result.Data);
    }

    [DataTestMethod]
    [DataRow(0, 10, "Invalid page number")]
    [DataRow(-1, 10, "Invalid page number")]
    [DataRow(1, 0, "Invalid page size")]
    [DataRow(1, -1, "Invalid page size")]
    public async Task Get_InvalidPaging_ShouldReturnBadRequest(int page, int pageSize, string expectedMessage)
    {
        // Arrange / Act
        var result = await _service.Get(page, pageSize);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
        Assert.AreEqual(expectedMessage, result.Error!.Message);
    }

    [TestMethod]
    public async Task Get_ValidPaging_ShouldReturnMappedAsyncDtos()
    {
        // Arrange
        var entities = new[] { TestData.ScopeEntity(), TestData.ScopeEntity(TestData.OtherScopeId, "Write Users", "write users", "users:write") };
        _repositoryMock.Setup(x => x.GetAllAsync(1, 10, default)).ReturnsAsync(entities);
        _mapperMock.Setup(x => x.Map<IEnumerable<ScopeEntity>>(entities)).Returns(entities);
        _mapperMock.Setup(x => x.Map<ScopeDto>(entities[0])).Returns(TestData.ScopeDto());
        _mapperMock.Setup(x => x.Map<ScopeDto>(entities[1])).Returns(TestData.ScopeDto(TestData.OtherScopeId, "Write Users", "users:write"));

        // Act
        var result = await _service.Get(1, 10);
        var items = await TestData.ToListAsync(result.Data!);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreEqual(2, items.Count);
        Assert.AreEqual("Write Users", items[1].ScopeName);
    }

    [DataTestMethod]
    [DataRow(0, 10, "Invalid page number")]
    [DataRow(1, 0, "Invalid page size")]
    public async Task Search_InvalidPaging_ShouldReturnBadRequest(int page, int size, string expectedMessage)
    {
        // Arrange / Act
        var result = await _service.Search("users", page, size);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
        Assert.AreEqual(expectedMessage, result.Error!.Message);
    }

    [TestMethod]
    public async Task Search_ValidTerm_ShouldReturnMappedDtos()
    {
        // Arrange
        var entities = new[] { TestData.ScopeEntity() };
        var dtos = new[] { TestData.ScopeDto() };
        _repositoryMock.Setup(x => x.SearchAsync("users", 1, 5, default)).ReturnsAsync(entities);
        _mapperMock.Setup(x => x.Map<IEnumerable<ScopeDto>>(entities)).Returns(dtos);

        // Act
        var result = await _service.Search("users", 1, 5);

        // Assert
        Assert.IsFalse(result.IsFailed);
        CollectionAssert.AreEqual(dtos, result.Data!.ToArray());
    }

    [TestMethod]
    public async Task Update_EmptyId_ShouldReturnBadRequest()
    {
        // Arrange / Act
        var result = await _service.Update(Guid.Empty, TestData.ScopeDto());

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
    }

    [TestMethod]
    public async Task Update_MissingScope_ShouldReturnNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, true, default)).ReturnsAsync((ScopeEntity?)null);

        // Act
        var result = await _service.Update(TestData.ScopeId, TestData.ScopeDto());

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.NotFound, result.ErrorType);
    }

    [TestMethod]
    public async Task Update_ConflictingName_ShouldReturnConflict()
    {
        // Arrange
        var existing = TestData.ScopeEntity();
        var conflicting = TestData.ScopeEntity(TestData.OtherScopeId);
        var dto = TestData.ScopeDto(name: "Write Users", value: null, description: null);
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, true, default)).ReturnsAsync(existing);
        _repositoryMock.Setup(x => x.GetByNameAsync("write users", default)).ReturnsAsync(conflicting);

        // Act
        var result = await _service.Update(TestData.ScopeId, dto);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.Conflict, result.ErrorType);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ScopeEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Update_ConflictingValue_ShouldReturnConflict()
    {
        // Arrange
        var existing = TestData.ScopeEntity();
        var conflicting = TestData.ScopeEntity(TestData.OtherScopeId);
        var dto = TestData.ScopeDto(name: null, value: "users:write", description: null);
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, true, default)).ReturnsAsync(existing);
        _repositoryMock.Setup(x => x.GetByValueAsync("users:write", default)).ReturnsAsync(conflicting);

        // Act
        var result = await _service.Update(TestData.ScopeId, dto);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.Conflict, result.ErrorType);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ScopeEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Update_SameNameAndValueConflictRecords_ShouldAllowUpdate()
    {
        // Arrange
        var existing = TestData.ScopeEntity();
        var dto = TestData.ScopeDto(name: "Read Users", value: "users:read", description: "Updated");
        var mappedDto = TestData.ScopeDto(description: "Updated");
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, true, default)).ReturnsAsync(existing);
        _repositoryMock.Setup(x => x.GetByNameAsync("read users", default)).ReturnsAsync(existing);
        _repositoryMock.Setup(x => x.GetByValueAsync("users:read", default)).ReturnsAsync(existing);
        _repositoryMock.Setup(x => x.UpdateAsync(existing, default)).ReturnsAsync(existing);
        _mapperMock.Setup(x => x.Map<ScopeDto>(existing)).Returns(mappedDto);

        // Act
        var result = await _service.Update(TestData.ScopeId, dto);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreEqual("Updated", existing.Description);
        Assert.AreSame(mappedDto, result.Data);
        _loggerMock.VerifyLogInformation(Times.Exactly(3));
    }

    [TestMethod]
    public async Task Update_OnlyDescription_ShouldUpdateDescription()
    {
        // Arrange
        var existing = TestData.ScopeEntity();
        var dto = TestData.ScopeDto(name: null, value: null, description: "New description");
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, true, default)).ReturnsAsync(existing);
        _repositoryMock.Setup(x => x.UpdateAsync(existing, default)).ReturnsAsync(existing);
        _mapperMock.Setup(x => x.Map<ScopeDto>(existing)).Returns(TestData.ScopeDto(description: "New description"));

        // Act
        var result = await _service.Update(TestData.ScopeId, dto);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreEqual("New description", existing.Description);
        Assert.AreEqual("New description", result.Data!.ScopeDescription);
    }

    [TestMethod]
    public async Task Update_NoFieldsProvided_ShouldReturnCurrentScopeAndSaveTrackedEntity()
    {
        // Arrange
        var existing = TestData.ScopeEntity();
        var dto = TestData.ScopeDto(name: " ", value: "", description: null);
        var mappedDto = TestData.ScopeDto();
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, true, default)).ReturnsAsync(existing);
        _repositoryMock.Setup(x => x.UpdateAsync(existing, default)).ReturnsAsync(existing);
        _mapperMock.Setup(x => x.Map<ScopeDto>(existing)).Returns(mappedDto);

        // Act
        var result = await _service.Update(TestData.ScopeId, dto);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreSame(mappedDto, result.Data);
        _repositoryMock.Verify(x => x.UpdateAsync(existing, default), Times.Once);
    }

    [TestMethod]
    public async Task Delete_EmptyId_ShouldReturnBadRequest()
    {
        // Arrange / Act
        var result = await _service.Delete(Guid.Empty);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.BadRequest, result.ErrorType);
    }

    [TestMethod]
    public async Task Delete_MissingScope_ShouldReturnNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, default)).ReturnsAsync((ScopeEntity?)null);

        // Act
        var result = await _service.Delete(TestData.ScopeId);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.NotFound, result.ErrorType);
    }

    [TestMethod]
    public async Task Delete_ExistingScope_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var entity = TestData.ScopeEntity();
        _repositoryMock.Setup(x => x.GetByIdAsync(TestData.ScopeId, default)).ReturnsAsync(entity);
        _repositoryMock.Setup(x => x.DeleteAsync(entity, default)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.Delete(TestData.ScopeId);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.IsTrue(result.Data);
        _loggerMock.VerifyLogInformation(Times.Once());
    }
}
