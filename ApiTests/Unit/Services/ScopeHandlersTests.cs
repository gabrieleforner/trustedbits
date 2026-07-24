using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Domain.Repository;
using Trustedbits.ApiServer.Infrastructure.Automapper;
using Trustedbits.ApiServer.Services;
using Trustedbits.ApiServer.Services.Handlers;
using Trustedbits.ApiServer.Services.Schemas;

namespace ApiTests.Unit.Services;

public class CreateScopeHandlerTests
{
    private Mock<IScopeRepository> _mockRepository;
    private ILogger<CreateScopeHandler> _logger;
    private IMapper _mapper;
    
    [SetUp]
    public void Setup()
    {
        _logger = NullLogger<CreateScopeHandler>.Instance;
        _mockRepository = new Mock<IScopeRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ScopeMapProfiles>();
        }, NullLoggerFactory.Instance);
        _mapper = new Mapper(config);   
    }
    
    [TearDown]
    public void TearDown()
    {
        _mockRepository.Reset();
    }

    // Validation tests
    [Test]
    public async Task TestValidationEmptyName_ShouldReturnBadRequestResult()
    {
        // Arrange test request
        var testRequest = new CreateScopeRequest
        {
            Name = string.Empty,
            Value = "test:test",
            Description = string.Empty
        };
        
        var handler = new  CreateScopeHandler(_mockRepository.Object, _mapper, NullLogger<CreateScopeHandler>.Instance);

        Result<CreateScopeResponse> response = await handler.Handle(testRequest, CancellationToken.None);

        // Verify that the service layer reports a failure, check if the error reason
        // it's a bad request, and check if the message contains the right attribute name
        Assert.That(response.Failed, Is.True);
        Assert.That(response.Data, Is.Null);
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error.Type, Is.EqualTo(ErrorType.BadRequest));
        Assert.That(response.Error.Message, Does.Contain("name"));
    }
    [Test]
    public async Task TestValidationEmptyValue_ShouldReturnBadRequestResult()
    {
        // Arrange test request
        var testRequest = new CreateScopeRequest
        {
            Name = "TestScope",
            Value = string.Empty,
            Description = string.Empty
        };
        
        var handler = new  CreateScopeHandler(_mockRepository.Object, _mapper, NullLogger<CreateScopeHandler>.Instance);
        Result<CreateScopeResponse> response = await handler.Handle(testRequest, CancellationToken.None);

        // Verify that the service layer reports a failure and check if the error reason
        // it's a bad request
        Assert.That(response.Failed, Is.True);
        Assert.That(response.Data, Is.Null);
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error.Type, Is.EqualTo(ErrorType.BadRequest));
        Assert.That(response.Error.Message, Does.Contain("value"));
    }
    [Test]
    public async Task TestValidationEmptyDescription_ShouldSucceed()
    {
        // Arrange the repository "Create" method
        _mockRepository.Setup(r => r.CreateScopeAsync(It.IsAny<Scope>(), It.IsAny<CancellationToken>()))
            .Returns<Scope, CancellationToken>((scope, ct) => Task.FromResult(new Scope(scope.Id, scope.Name, scope.Value, scope.Description)));

        // Arrange the test request
        var testRequest = new CreateScopeRequest
        {
            Name = "TestScope",
            Value = "test:value",
            Description = string.Empty,
        };
        
        var handler = new  CreateScopeHandler(_mockRepository.Object, _mapper, NullLogger<CreateScopeHandler>.Instance);
        Result<CreateScopeResponse> response = await handler.Handle(testRequest, CancellationToken.None);
       
        // Assert that the service has run successfully without producing any error
        Assert.That(response.Failed, Is.False);
        Assert.That(response.Data, Is.Not.Null);
        Assert.That(response.Error, Is.Null);
        
        // Assert that the name, value, and description are unchanged
        Assert.That(response.Data.Name, Is.EqualTo(testRequest.Name));
        Assert.That(response.Data.Value, Is.EqualTo(testRequest.Value));
        Assert.That(response.Data.Description, Is.EqualTo(testRequest.Description));
    }
    

    // Conflict Tests
    [Test]
    public async Task TestConflictingName_ShouldReturnConflictResult()
    {
        // Arrange the repository "ExistsByName" method to always return a conflict
        _mockRepository.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((name, ct) => Task.FromResult(true));
        
        // Arrange the test request
        var testRequest = new CreateScopeRequest()
        {
            Name = "TestScope",
            Value = "test:value",
            Description = string.Empty,
        };
        
        var handler =  new CreateScopeHandler(_mockRepository.Object, _mapper, NullLogger<CreateScopeHandler>.Instance);
        var result = await handler.Handle(testRequest, CancellationToken.None);
        
        
        // Verify that the service layer reports a failure
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Failed, Is.True);
        Assert.That(result.Error, Is.Not.Null);
        
        // check if the error reason it's an "already exists", and check if the message contains the right attribute name
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.AlreadyExists));
        Assert.That(result.Error.Message, Does.Contain("name"));
    }
    [Test]
    public async Task TestConflictingValue_ShouldReturnConflictResult()
    {
        // Arrange the repository "ExistsByValue" method to always return a conflict
        _mockRepository.Setup(r => r.ExistsByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((name, ct) => Task.FromResult(true));
        
        // Arrange the test request
        var testRequest = new CreateScopeRequest()
        {
            Name = "TestScope",
            Value = "test:value",
            Description = string.Empty,
        };
        
        var handler =  new CreateScopeHandler(_mockRepository.Object, _mapper, NullLogger<CreateScopeHandler>.Instance);
        var result = await handler.Handle(testRequest, CancellationToken.None);
        
        
        // Verify that the service layer reports a failure
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Failed, Is.True);
        Assert.That(result.Error, Is.Not.Null);
        
        // check if the error reason it's an "already exists", and check if the message contains the right attribute name
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.AlreadyExists));
        Assert.That(result.Error.Message, Does.Contain("value"));
    }

    // Resilience test
    [Test]
    public async Task TestDbFailure_ShouldReturnServerErrorResult ()
    {
        // Arrange the repository so "ExistsByName" method always return a DB-side error (e.g. a connection failure)
        _mockRepository.Setup(r => r.ExistsByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new RepositoryException("DB connection failure", RepositoryExceptionType.ServerException));
        
        // Arrange the test request
        var testRequest = new CreateScopeRequest()
        {
            Name = "TestScope",
            Value = "test:value",
            Description = string.Empty,
        };
        
        var handler =  new CreateScopeHandler(_mockRepository.Object, _mapper, NullLogger<CreateScopeHandler>.Instance);
        var result = await handler.Handle(testRequest, CancellationToken.None);
        
        
        // Verify that the service layer reports a failure
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Failed, Is.True);
        Assert.That(result.Error, Is.Not.Null);
        
        // check if the error reason it's an internal server error
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.ServerError));
    }
}

public class DescribeScopeHandlerTests
{
    private Mock<IScopeRepository> _mockRepository;
    private ILogger<CreateScopeHandler> _mockLogger;
    private IMapper _mapper;
    
    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ScopeMapProfiles>();
        }, NullLoggerFactory.Instance);
        _mapper = new Mapper(config);
        _mockLogger = NullLoggerFactory.Instance.CreateLogger<CreateScopeHandler>();
        _mockRepository = new Mock<IScopeRepository>();
    }

    [Test]
    public async Task TestValidationEmptyId_ShouldReturnBadRequestResult() 
    {
        var testRequest = new DescribeScopeRequest { Id = Guid.Empty };
        var handler = new DescribeScopeHandler(_mockRepository.Object, NullLogger<DescribeScopeHandler>.Instance, _mapper);
        var response = await handler.Handle(testRequest, CancellationToken.None);
        
        Assert.That(response.Data, Is.Null);
        Assert.That(response.Failed, Is.True);
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error.Type, Is.EqualTo(ErrorType.BadRequest));
    }
    
    [Test]
    public async Task TestDbFailure_ShouldReturnServerError() 
    {
        _mockRepository.Setup(r => r.ExistsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Throws(new RepositoryException("DB connection failure", RepositoryExceptionType.ServerException));

        var testRequest = new DescribeScopeRequest { Id = Guid.NewGuid() };
        var handler = new DescribeScopeHandler(_mockRepository.Object, NullLogger<DescribeScopeHandler>.Instance, _mapper);
        var response = await handler.Handle(testRequest, CancellationToken.None);
        
        Assert.That(response.Data, Is.Null);
        Assert.That(response.Failed, Is.True);
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error.Type, Is.EqualTo(ErrorType.ServerError));
    }
    
    [TearDown]
    public void TearDown()
    {
        _mockRepository.Reset();
    }
}