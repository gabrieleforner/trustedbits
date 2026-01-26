using Trustedbits.ApiServer.Models.DTOs;

namespace Trustedbits.ApiServer.Services;

/// <summary>
/// All possible results that an authentication request
/// can gie in response.
/// </summary>
public enum AuthenticationStatus
{   
    AuthenticationSucceded,
    InvalidCredentials,
    TenantNotFound,
}
public record AuthenticationResult(AuthenticationStatus Status, object? ResultBody);


/// <summary>
/// Describes what methods must be implemented by an authentication
/// service implementation
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Log in a user using data from the DTO (<see cref="UserLoginRequestDto"/>
    /// </summary>
    /// <param name="loginData">DTO instance with login data pre-validated</param>
    /// <param name="tenantId">ID of the tenant where the user may belong</param>
    /// <returns>An <see cref="AuthenticationResult"/> describing the status of the authentication</returns>
    Task<AuthenticationResult> Login(UserLoginRequestDto loginData, Guid tenantId);
    
    /// <summary>
    /// Sign up a user
    /// </summary>
    /// <param name="signupData">DTO containing data to sign up the user</param>
    /// <param name="tenantId">ID of the tenant where the user wants to sign up</param>
    /// <returns>An <see cref="AuthenticationResult"/> describing the status of the signup request</returns>
    Task<AuthenticationResult> Signup(UserSignupRequestDto signupData, Guid tenantId);
}