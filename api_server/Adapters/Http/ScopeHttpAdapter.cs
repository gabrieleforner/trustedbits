using Microsoft.AspNetCore.Mvc;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;

namespace Trustedbits.ApiServer.Adapters.Http;

[ApiController]
[Route("/api/scopes")]
public class ScopeHttpAdapter : ControllerBase
{
    private IScopeService _scopeService;

    public ScopeHttpAdapter(IScopeService scopeService)
    {
        _scopeService = scopeService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ScopeDto scope)
    {
        var result = await _scopeService.CreateScope(scope);
        if (result.IsFailed)
        {
            switch (result.ErrorType)
            {
                case ErrorType.BadRequest:
                    return BadRequest(result.Error);
                case ErrorType.Conflict:
                    return Conflict(result.Error);
            }
        }
        
        return CreatedAtAction(nameof(GetById), new { id = result.Data.ScopeId }, result.Data);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _scopeService.GetScope(id);
        if (result.IsFailed)
        {
            switch (result.ErrorType)
            {
                case ErrorType.BadRequest:
                    return BadRequest(result.Error);
                case ErrorType.NotFound:
                    return NotFound(result.Error);
            }
        }
        return Ok(result.Data);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _scopeService.GetAllScopes(page, pageSize);
        if (result.IsFailed)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ScopeDto scope)
    {
        var result = await _scopeService.UpdateScope(id, scope);
        if (result.IsFailed)
        {
            switch (result.ErrorType)
            {
                case ErrorType.BadRequest:
                    return BadRequest(result.Error);
                case ErrorType.NotFound:
                    return NotFound(result.Error);
                case ErrorType.Conflict:
                    return Conflict(result.Error);
            }
        }
        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _scopeService.DeleteScope(id);
        if (result.IsFailed)
        {
            switch (result.ErrorType)
            {
                case ErrorType.BadRequest:
                    return BadRequest(result.Error);
                case ErrorType.NotFound:
                    return NotFound(result.Error);
            }
        }
        return Ok(result.Data);
    }
}