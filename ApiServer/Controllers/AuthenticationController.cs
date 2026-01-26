using Microsoft.AspNetCore.Mvc;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Services;

namespace Trustedbits.ApiServer.Controllers;

[ApiController]
[Route("{tenantId:guid}/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginEndpointController([FromRoute] Guid tenantId, UserLoginRequestDto loginData)
    {
        var loginResult = await _authenticationService.Login(loginData, tenantId);
        if (loginResult.Status != AuthenticationStatus.AuthenticationSucceded)
        {
            return BadRequest(loginResult.Status.ToString());
        }
        return Ok(loginResult.ResultBody);
    }
}