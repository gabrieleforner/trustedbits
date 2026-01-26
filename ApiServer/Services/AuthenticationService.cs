using Microsoft.IdentityModel.Tokens;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Delegates;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;

namespace Trustedbits.ApiServer.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRepository<Tenant> _tenantsRepository;
    private readonly ISessionDelegate _sessionDelegate;
    private readonly IAuthenticationDelegate _authenticationDelegate;
    
    public AuthenticationService(IRepository<Tenant> tenantsRepository, ISessionDelegate sessionDelegate,
        IAuthenticationDelegate authenticationDelegate)
    {
        _tenantsRepository = tenantsRepository;
        _sessionDelegate = sessionDelegate;
        _authenticationDelegate = authenticationDelegate;
    }
    
    public async Task<AuthenticationResult> Login(UserLoginRequestDto loginData, Guid tenantId)
    {
        // Verify the requested tenant exists
        var matchingTenant = (await _tenantsRepository.GetAsync(t => t.Id == tenantId)).ToList();
        if (matchingTenant.IsNullOrEmpty())
            return new(AuthenticationStatus.TenantNotFound, null);
        
        // Call delegate to authenticate user
        var sessionIdKeyEmail = await _authenticationDelegate.AuthenticateUser(loginData, tenantId);
        if(sessionIdKeyEmail == null)
            return new(AuthenticationStatus.InvalidCredentials, null);
        
        // Initialise IdP session
        var sessionId = await _sessionDelegate.CreateIdPSession(tenantId, sessionIdKeyEmail);
        return new AuthenticationResult(AuthenticationStatus.AuthenticationSucceded, new
        {
            SessionId = sessionId
        });
    }

    public async Task<AuthenticationResult> Signup(UserSignupRequestDto signupData, Guid tenantId)
    {
        throw new NotImplementedException();
    }
}