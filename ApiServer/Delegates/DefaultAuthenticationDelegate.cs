using Microsoft.AspNetCore.Identity;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;

namespace Trustedbits.ApiServer.Delegates;

/// <summary>
/// Default implementation of <see cref="IAuthenticationDelegate"/> that relies on an <see cref="IRepository{TEntity}"/>
/// and the ASP.NET Identity Framework Core in order to assert user identity.
/// </summary>
public class DefaultAuthenticationDelegate : IAuthenticationDelegate
{
    private readonly UserManager<User> _userManager;
    private readonly IRepository<User> _userRepository;
    
    /// <summary>
    /// Initialize the authentication provider.
    /// </summary>
    /// <param name="userManager">Instance of <see cref="UserManager{TUser}"/></param>
    /// <param name="userRepository">Instance of <see cref="IRepository{TEntity}"/></param>
    public DefaultAuthenticationDelegate(UserManager<User> userManager, IRepository<User> userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }
    
    /// <inheritdoc/>
    public async Task<bool> AuthenticateUser(UserLoginRequestDto loginDto, Guid tenantId)
    {
        // Check if whether the account exists in the requested tenant
        User? matchingUser = await GetMatchingUser(loginDto, tenantId);
        if (matchingUser == null)
            return false;
        
    
        var isPasswordValid = await _userManager.CheckPasswordAsync(matchingUser, loginDto.Password);
        if (!isPasswordValid)
            return false;
        return true;
    }

    /// <summary>
    /// Returns (if present) the database entry (mapped as a class instance) of a user that matches
    /// login data and tenant ID.
    /// </summary>
    /// <param name="loginDto"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    private async Task<User?> GetMatchingUser(UserLoginRequestDto loginDto, Guid tenantId)
    {
        var matching = await _userRepository.GetAsync(u =>
            u.ParentTenantId == tenantId &&
            
            ((!string.IsNullOrEmpty(loginDto.Email) &&
              u.Email == loginDto.Email) ||

             (!string.IsNullOrEmpty(loginDto.Username) &&
              u.UserName == loginDto.Username) ||

             (!string.IsNullOrEmpty(loginDto.PhoneNumber) &&
              u.PhoneNumber == loginDto.PhoneNumber))
        );
        
        return matching?.FirstOrDefault();
    }
}