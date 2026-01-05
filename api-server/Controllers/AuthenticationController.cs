using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using TrustedbitsApiServer.Models;

namespace TrustedbitsApiServer.Controllers;


[ApiController]
[Route("/{tenantId}")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IDistributedCache _sessionsCache;
    private readonly UserManager<User> _userManager;
    
    [HttpPost]
    [Route("/{tenantId:guid}/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, [FromRoute] Guid requestTenantId)
    {
        try
        {
            // Find the user
            User? matchingUser = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (matchingUser == null)
            {
                return Unauthorized(new
                {
                    Error = "ERR_INVALID_CREDENTIALS",
                    ErrorDescription = "Wrong email/password"
                });
            }

            if (matchingUser.TenantId != requestTenantId)
            {
                return Unauthorized(new
                {
                    Error = "ERR_INVALID_CREDENTIALS",
                    ErrorDescription = "Invalid tenant Id"
                });
            }

            // Validate user password
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(matchingUser, loginRequest.Password);
            if (!isPasswordCorrect)
            {
                return Unauthorized(new
                {
                    Error = "ERR_INVALID_CREDENTIALS",
                    ErrorDescription = "Wrong email/password"
                });
            }

            // Create a new session
            var sessionId = Guid.NewGuid().ToString();
            var cachingOptions = new DistributedCacheEntryOptions();
            cachingOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            
            await _sessionsCache.SetStringAsync(
                $"idp_session:{matchingUser.TenantId}:{matchingUser.Id}", 
                sessionId,
                cachingOptions);
            
            return Ok(new
            {
                SessionId = sessionId,
                UserId = matchingUser.Id
            });
        }
        catch (Exception e)
        {
            // In case any error is thrown by the underlying services (Redis, SQL Server, ...)
            _logger.LogError(e.ToString());
            return Problem(title: "Internal server error", 
                detail: "Something went wrong while logging in.",
                statusCode: 500);
        }
    }
    
    [HttpPost]
    [HttpGet]
    [Route("/logout")]
    public async Task<IActionResult> Logout()
    {
        var accessToken = HttpContext.Request.Headers["Authorization"].ToString();
        if (accessToken.IsNullOrEmpty() || !accessToken.StartsWith("Bearer "))
        {
            return Unauthorized();
        }
        accessToken = accessToken.Substring("Bearer ".Length);
        
        
        
        return Ok();
    }
}