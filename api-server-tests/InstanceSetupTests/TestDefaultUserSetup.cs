using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrustedbitsApiServer.Models;
using TrustedbitsApiServer.Services;

namespace ApiServerTests.InstanceSetupTests;

public class TestDefaultUserSetup
{
    private Mock<ILogger> _mockLogger;
    private ServerDbContext _dbContext;
    private Mock<UserManager<User>> _mockUserManager;
    private Mock<RoleManager<Role>> _mockRoleManager;
    private InstanceSetup _instanceSetup;
    private Guid _defaultTenantId;

    [SetUp]
    public void Setup()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ServerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ServerDbContext(options);

        // Setup mocks
        _mockLogger = new Mock<ILogger>();
        
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        var roleStore = new Mock<IRoleStore<Role>>();
        _mockRoleManager = new Mock<RoleManager<Role>>(
            roleStore.Object, null, null, null, null);

        // Create default tenant
        _defaultTenantId = Guid.NewGuid();
        var defaultTenant = new Tenant
        {
            Id = _defaultTenantId,
            Name = InstanceSetup.DefaultTenantName
        };
        _dbContext.Tenants.Add(defaultTenant);
        _dbContext.SaveChanges();

        // Setup UserManager.Users to return queryable collection
        var users = new List<User>().AsQueryable();
        _mockUserManager.Setup(x => x.Users).Returns(users);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Database.EnsureDeleted();
        _dbContext?.Dispose();
    }
    
    [Test]
    public void TestDefaultUserSetup_ShouldCreateUser()
    {
        // Arrange
        _mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _instanceSetup = new InstanceSetup(
            _mockLogger.Object,
            _dbContext,
            _mockUserManager.Object,
            _mockRoleManager.Object);

        // Setup tenant first
        _instanceSetup.SetupDefaultTenant();

        // Act
        _instanceSetup.SetupDefaultAccount();

        // Assert
        _mockUserManager.Verify(
            x => x.CreateAsync(
                It.Is<User>(u => 
                    u.UserName == InstanceSetup.DefaultUserName &&
                    u.Email == InstanceSetup.DefaultEmail &&
                    u.EmailConfirmed == true &&
                    u.TenantId == _defaultTenantId),
                InstanceSetup.DefaultPassword),
            Times.Once,
            "Should create default user with correct properties");
    }
    
    [Test]
    public void TestDefaultUserSetup_ShouldNotCreateUser()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = InstanceSetup.DefaultUserName,
            Email = InstanceSetup.DefaultEmail,
            TenantId = _defaultTenantId
        };

        var usersWithExisting = new List<User> { existingUser }.AsQueryable();
        _mockUserManager.Setup(x => x.Users).Returns(usersWithExisting);

        _instanceSetup = new InstanceSetup(
            _mockLogger.Object,
            _dbContext,
            _mockUserManager.Object,
            _mockRoleManager.Object);

        // Setup tenant first
        _instanceSetup.SetupDefaultTenant();

        // Act
        _instanceSetup.SetupDefaultAccount();

        // Assert
        _mockUserManager.Verify(
            x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()),
            Times.Never,
            "Should not create default user when it already exists");
    }

    [Test]
    public void TestDefaultUserSetup_ShouldThrowOnFailure()
    {
        // Arrange
        var errors = new[]
        {
            new IdentityError { Description = "Password too weak" },
            new IdentityError { Description = "Username already taken" }
        };
        var failedResult = IdentityResult.Failed(errors);

        _mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(failedResult);

        _instanceSetup = new InstanceSetup(
            _mockLogger.Object,
            _dbContext,
            _mockUserManager.Object,
            _mockRoleManager.Object);

        // Setup tenant first
        _instanceSetup.SetupDefaultTenant();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _instanceSetup.SetupDefaultAccount());
        
        Assert.That(exception.Message, Does.Contain("Failed to create default account"));
        Assert.That(exception.Message, Does.Contain("Password too weak"));
        Assert.That(exception.Message, Does.Contain("Username already taken"));
    }
}