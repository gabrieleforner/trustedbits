using Microsoft.AspNetCore.Mvc;

namespace TrustedbitsApiServer.Controllers;

[Route("/{tenantId}/openid")]
public class OpenIdController : ControllerBase
{
    [HttpGet]
    [Route("/{tenantId}/openid/authorize")]
    public async Task<IActionResult> Authorize(string tenantId) {}
    
    [HttpPost]
    [Route("/{tenantId}/openid/token")]
    public async Task<IActionResult> Token(string tenantId) {}
    
    [HttpGet]
    [Route("/{tenantId}/openid/jwks.json")]
    public async Task<IActionResult> TokenPublicKeys(string tenantId) {}
    
    [HttpGet]
    [Route("/{tenantId}/openid/userinfo")]
    public async Task<IActionResult> UserInfo(string tenantId) {}
}