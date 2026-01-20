using Trustedbits.ApiServer.Models.DTOs;

namespace Trustedbits.ApiServer.Delegates;

/// <summary>
/// This interface defines what methods must be exposed by a class
/// that provides authentication features.
/// </summary>
public interface IAuthenticationDelegate
{
    /// <summary>
    /// Verifies whether a user exists in that same tenant.
    /// </summary>
    /// <param name="loginDto"><see cref="UserLoginRequestDto"/> containing login information</param>
    /// <param name="tenantId">ID of the tenant provided by the client in the URL path</param>
    /// <returns>Whether is the user exists and its credentials match or not</returns>
    Task<bool> AuthenticateUser(UserLoginRequestDto loginDto, Guid tenantId);
}