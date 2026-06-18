using Microsoft.AspNetCore.Mvc;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;

namespace Trustedbits.ApiServer.Adapters.Http;

/// <summary>
/// HTTP adapter exposing REST endpoints for scope operations.
/// Delegates business logic to an <see cref="IScopeService"/> instance.
/// </summary>
[ApiController]
[Route("/api/scopes")]
public class ScopeHttpAdapter : ControllerBase
{
    /// <summary>
    /// Instance of the service layer
    /// </summary>
    private IScopeService _scopeService;

    /// <summary>
    /// Creates a new instance of the <see cref="ScopeHttpAdapter"/>.
    /// </summary>
    /// <param name="scopeService">Service used to perform scope operations.</param>
    public ScopeHttpAdapter(IScopeService scopeService)
    {
        _scopeService = scopeService;
    }

    /// <summary>
    /// Creates a new scope.
    /// </summary>
    /// <param name="scope">The scope DTO payload supplied in the request body.</param>
    /// <returns>
    /// 201 Created with the created resource on success; 400 Bad Request or 409 Conflict when the operation fails.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ScopeDto scope)
    {
        var result = await _scopeService.Create(scope);
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
        
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.ScopeId }, result.Data);
    }
    
    /// <summary>
    /// Retrieves a scope by its identifier.
    /// </summary>
    /// <param name="id">The GUID identifier of the scope.</param>
    /// <returns>200 OK with the scope DTO, or 400/404 on failure.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _scopeService.Get(id);
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
    
    /// <summary>
    /// Retrieves a paginated list of scopes.
    /// </summary>
    /// <param name="page">One-based page index (default 1).</param>
    /// <param name="pageSize">Page size (default 10).</param>
    /// <returns>200 OK with a collection of scope DTOs, or 400 Bad Request.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _scopeService.Get(page, pageSize);
        if (result.IsFailed)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Data);
    }
    
    /// <summary>
    /// Updates an existing scope.
    /// </summary>
    /// <param name="id">Identifier of the scope to update.</param>
    /// <param name="scope">Payload containing updated scope fields.</param>
    /// <returns>200 OK with the updated DTO, or 400/404/409 on error.</returns>
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ScopeDto scope)
    {
        var result = await _scopeService.Update(id, scope);
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

    /// <summary>
    /// Deletes a scope by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the scope to delete.</param>
    /// <returns>200 OK with the deletion result or 400/404 on error.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _scopeService.Delete(id);
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