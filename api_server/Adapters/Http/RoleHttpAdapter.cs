using Microsoft.AspNetCore.Mvc;
using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;

namespace Trustedbits.ApiServer.Adapters.Http;

/// <summary>
/// HTTP adapter exposing REST endpoints for scope operations.
/// Delegates business logic to an <see cref="IRoleService"/> instance.
/// </summary>
[ApiController]
[Route("/api/roles")]
public class RoleHttpAdapter : ControllerBase
{
    private readonly IRoleService _service;

    /// <summary>
    /// Constructor for HTTP controller.
    /// </summary>
    /// <param name="service"></param>
    public RoleHttpAdapter(IRoleService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// HTTP adapter for expose <c>IRoleService.Create</c> use case
    /// </summary>
    /// <param name="dto">DTO with new role data</param>
    /// <returns>201, else 409 in case of conflict or 400 in case of bad request formats</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleDto dto)
    {
        var result = await _service.Create(dto);
        if (result.IsFailed)
        {
            switch (result.ErrorType)
            {
                case ErrorType.Conflict:
                    return Conflict(result.Error);
                case ErrorType.BadRequest:
                    return BadRequest(result.Error);
            }
        }
        return CreatedAtAction(nameof(Get), new { id = result.Data!.RoleId },  result.Data);
    }
    
    /// <summary>
    /// HTTP adapter for expose <c>IRoleService.Get(id)</c> use case
    /// </summary>
    /// <param name="id">ID of the role to describe</param>
    /// <returns>200, else 404 in case ID cloud not be found or 400 in case of bad request formats</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await _service.Get(id);
        if (result.IsFailed)
        {
            switch (result.ErrorType)
            {
                case ErrorType.NotFound:
                    return NotFound(result.Error);
                case ErrorType.BadRequest:
                    return BadRequest(result.Error);
            }
        }
        return Ok(result.Data);
    }
    
    /// <summary>
    /// HTTP adapter for expose <c>IRoleService.Get(page, pageSize)</c> use case
    /// </summary>
    /// <param name="page">Number of page</param>
    /// <param name="pageSize">Number of elements per page</param>
    /// <returns>200, else 404 in case ID cloud not be found or 400 in case of bad request formats</returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page, int pageSize)
    {
        var result = await _service.Get(page, pageSize);
        if (result.IsFailed)
            return BadRequest(result.Error);
        
        return Ok(result.Data);
    }

}