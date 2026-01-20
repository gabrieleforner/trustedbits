using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Delegates;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using System.Linq.Expressions;

namespace Trustedbits.ApiServerTests.AuthenticationProvider;

[TestFixture]
public class DefaultAuthenticationDelegateTests
{
    private Mock<UserManager<User>> _mockUserManager;
    private Mock<IRepository<User>> _mockUserRepository;
    private DefaultAuthenticationDelegate _authenticationDelegate;
    private Guid _testTenantId;

    [SetUp]
    public void Setup()
    {
        // Mock UserManager (requires mocking the store)
        var mockUserStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            mockUserStore.Object,
            null, null, null, null, null, null, null, null);

        _mockUserRepository = new Mock<IRepository<User>>();
        
        _authenticationDelegate = new DefaultAuthenticationDelegate(
            _mockUserManager.Object,
            _mockUserRepository.Object);

        _testTenantId = Guid.NewGuid();
    }

    [Test]
    public async Task AuthenticateUser_WithValidEmailAndPassword_ReturnsTrue()
    {
        // Arrange
        var loginDto = new UserLoginRequestDto
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        var user = new User
        {
            Id = new Guid(Guid.NewGuid().ToString()),
            Email = "test@example.com",
            UserName = "testuser",
            ParentTenantId = _testTenantId
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        _mockUserManager
            .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, _testTenantId);

        // Assert
        Assert.That(result, Is.True);
        _mockUserRepository.Verify(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(user, loginDto.Password), Times.Once);
    }

    [Test]
    public async Task AuthenticateUser_WithValidUsernameAndPassword_ReturnsTrue()
    {
        // Arrange
        var loginDto = new UserLoginRequestDto
        {
            Username = "testuser",
            Password = "ValidPassword123!"
        };

        var user = new User
        {
            Id = new Guid(Guid.NewGuid().ToString()),
            Email = "test@example.com",
            UserName = "testuser",
            ParentTenantId = _testTenantId
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        _mockUserManager
            .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, _testTenantId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AuthenticateUser_WithValidPhoneNumberAndPassword_ReturnsTrue()
    {
        // Arrange
        var loginDto = new UserLoginRequestDto
        {
            PhoneNumber = "+1234567890",
            Password = "ValidPassword123!"
        };

        var user = new User
        {
            Id = new Guid(Guid.NewGuid().ToString()),
            PhoneNumber = "+1234567890",
            UserName = "testuser",
            ParentTenantId = _testTenantId
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        _mockUserManager
            .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, _testTenantId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AuthenticateUser_WithInvalidPassword_ReturnsFalse()
    {
        // Arrange
        var loginDto = new UserLoginRequestDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var user = new User
        {
            Id = new Guid(Guid.NewGuid().ToString()),
            Email = "test@example.com",
            UserName = "testuser",
            ParentTenantId = _testTenantId
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        _mockUserManager
            .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, _testTenantId);

        // Assert
        Assert.That(result, Is.False);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(user, loginDto.Password), Times.Once);
    }

    [Test]
    public async Task AuthenticateUser_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        var loginDto = new UserLoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, _testTenantId);

        // Assert
        Assert.That(result, Is.False);
        _mockUserRepository.Verify(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task AuthenticateUser_WithWrongTenantId_ReturnsFalse()
    {
        // Arrange
        var wrongTenantId = Guid.NewGuid();
        var loginDto = new UserLoginRequestDto
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, wrongTenantId);

        // Assert
        Assert.That(result, Is.False);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task AuthenticateUser_WithNullUserFromRepository_ReturnsFalse()
    {
        // Arrange
        var loginDto = new UserLoginRequestDto
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync((List<User>?)null);

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, _testTenantId);

        // Assert
        Assert.That(result, Is.False);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task AuthenticateUser_WithEmptyCredentials_ReturnsFalse()
    {
        // Arrange
        var loginDto = new UserLoginRequestDto
        {
            Email = "",
            Username = "",
            PhoneNumber = "",
            Password = "Password123!"
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, _testTenantId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task AuthenticateUser_WithMultipleMatchingUsers_UsesFirstUser()
    {
        // Arrange
        var loginDto = new UserLoginRequestDto
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        var user1 = new User
        {
            Id = new Guid(Guid.NewGuid().ToString()),
            Email = "test@example.com",
            UserName = "testuser1",
            ParentTenantId = _testTenantId
        };

        var user2 = new User
        {
            Id = new Guid(Guid.NewGuid().ToString()),
            Email = "test@example.com",
            UserName = "testuser2",
            ParentTenantId = _testTenantId
        };

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user1, user2 });

        _mockUserManager
            .Setup(um => um.CheckPasswordAsync(user1, loginDto.Password))
            .ReturnsAsync(true);

        // Act
        var result = await _authenticationDelegate.AuthenticateUser(loginDto, _testTenantId);

        // Assert
        Assert.That(result, Is.True);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(user1, loginDto.Password), Times.Once);
        _mockUserManager.Verify(um => um.CheckPasswordAsync(user2, It.IsAny<string>()), Times.Never);
    }
}