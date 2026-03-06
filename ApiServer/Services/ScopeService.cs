using System.Text.RegularExpressions;
using AutoMapper;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Abstract;
using Trustedbits.ApiServer.Services.Interfaces;

namespace Trustedbits.ApiServer.Services;

/// <inheritdoc/>
public class ScopeService : ScopeServiceBase
{
    /// <summary>
    /// Regular Expression used to validate the format of the scope value
    /// </summary>
    private const string ScopeRegex = "^[A-Za-z]+:[A-Za-z]+$";
    
    /// <summary>
    /// Instance of the DB abstraction for the scopes table
    /// </summary>
    private readonly IRepository<Scope> _scopeRepository;

    /// <summary>
    /// Instance of AutoMapper
    /// </summary>
    private readonly IMapper _objectMapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scopeRepository">Instance of <see cref="IRepository{Scope}"/></param>
    /// <param name="objectMapper">Instance of AutoMapper</param>
    public ScopeService(IRepository<Scope> scopeRepository, IMapper objectMapper)
    {
        _scopeRepository = scopeRepository;
        _objectMapper = objectMapper;
    }

    /// <inheritdoc/>
    public override async Task<ScopeServiceResult<ScopeDto>> CreateScope(Guid tenantId, ScopeDto scopeDto)
    {
        // Validate scope value
        if(string.IsNullOrWhiteSpace(scopeDto.ScopeValue))
            return new ScopeServiceResult<ScopeDto>(ScopeErrors.ScopeInvalidData,
                new ServiceError("ERR_INVALID_SCOPE_VALUE", "You must provide a non-blank scope value"));
        // Validate scope name
        if (string.IsNullOrWhiteSpace(scopeDto.ScopeName))
            scopeDto.ScopeName = scopeDto.ScopeValue;
        
        // Vefiy if RegEx is respected
        if(!Regex.IsMatch(scopeDto.ScopeValue, ScopeRegex))
            return new ScopeServiceResult<ScopeDto>(ScopeErrors.ScopeInvalidData,
                new ServiceError("ERR_INVALID_SCOPE_VALUE",
                    "You must provide a valid scope value (in form resource:verb)"));
        
        var mappedScope = _objectMapper.Map<Scope>(scopeDto);
        
        try
        {
            // Verify if the scope already exists
            mappedScope.ParentTenantId = tenantId;
            var matching = await _scopeRepository.Get(mappedScope.Id);
            if (matching != null)
                return new ScopeServiceResult<ScopeDto>(ScopeErrors.ScopeAlreadyExists,
                    new ServiceError("SCOPE_ALREADY_EXISTS", "Scope already exists"));
            
            // Create the new scope
            var result = await _scopeRepository.Create(mappedScope);
            return new ScopeServiceResult<ScopeDto>(_objectMapper.Map<ScopeDto>(result));
        }
        catch (Exception e)
        {
            // Handle DB errors (return 500)
            Console.WriteLine($"Service fail");
            Console.WriteLine($"\tMessage: {e.Message}");
            Console.WriteLine($"\tStack Frame: {e.StackTrace}");
            
            return new ScopeServiceResult<ScopeDto>(ScopeErrors.ServerError, 
                new ServiceError("INTERNAL_ERROR", "An unknown error occured while trying to create a new scope."));
        }
    }

    /// <inheritdoc/>
    public override async Task<ScopeServiceResult<List<ScopeDto>>> GetAllScopes(Guid tenantId, int page, int pageSize)
    {
        if (page < 1 )
            return new ScopeServiceResult<List<ScopeDto>>(ScopeErrors.ScopeInvalidData,
                new ServiceError("ERR_INVALID_PAGE_NUM", "You must provide a positive page number."));
        if(pageSize < 1)
            return new ScopeServiceResult<List<ScopeDto>>(ScopeErrors.ScopeInvalidData,
                new ServiceError("ERR_INVALID_PAGE_SIZE", "You must provide a positive page size."));

        try
        {
            var matching = await _scopeRepository.Get(s => s.ParentTenantId == tenantId);
            var mappedScopes = _objectMapper.Map<List<ScopeDto>>(matching);
            return new ScopeServiceResult<List<ScopeDto>>(mappedScopes);
        }
        catch (Exception e)
        {
            // Handle DB errors (return 500)
            Console.WriteLine($"Service fail");
            Console.WriteLine($"\tMessage: {e.Message}");
            Console.WriteLine($"\tStack Frame: {e.StackTrace}");
            
            return new ScopeServiceResult<List<ScopeDto>>(ScopeErrors.ServerError, 
                new ServiceError("INTERNAL_ERROR", "An unknown error occured while trying to create a new scope."));
        }
    }

    //TODO: Implement me
    /// <inheritdoc/>
    public override async Task<ScopeServiceResult<List<ScopeDto>>> GetScope(Guid tenantId, ScopeQueryDto scopeQueryData)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public override async Task<ScopeServiceResult<ScopeDto>> EditScope(Guid tenantId, string scopeName, ScopeDto scopeEditData)
    {
        if (string.IsNullOrEmpty(scopeName))
            return new ScopeServiceResult<ScopeDto>(ScopeErrors.ScopeInvalidData,
                new ServiceError("ERR_MISSING_DATA", "You must provide a non-blank scope name."));
                
        try
        {
            var matching = await _scopeRepository
                .FirstOrDefault(s => scopeName == s.Name && tenantId == s.ParentTenantId);
            if (matching == null)
                return new ScopeServiceResult<ScopeDto>(ScopeErrors.ScopeNotFound,
                    new ServiceError("ERR_SCOPE_NOT_FOUND", $"No such scope with name {scopeName} exists."));

            matching.Name = scopeEditData.ScopeName ?? matching.Name;
            matching.Description =  scopeEditData.ScopeDescription ?? matching.Description;
            matching.IsActive = scopeEditData.IsActive ?? matching.IsActive;
            
            await _scopeRepository.UpdateEntity(matching);

            var mapped = _objectMapper.Map<ScopeDto>(matching);
            return new ScopeServiceResult<ScopeDto>(mapped);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Service fail");
            Console.WriteLine($"\tMessage: {e.Message}");
            Console.WriteLine($"\tStack Frame: {e.StackTrace}");
            
            return new ScopeServiceResult<ScopeDto>(ScopeErrors.ServerError,
                new ServiceError("INTERNAL_ERROR", "You must provide a non-blank scope name."));
        }
    }

    /// <inheritdoc/>
    public override async Task<ScopeServiceResult<bool>> DeleteScope(Guid tenantId, string scopeName)
    {
        if (string.IsNullOrEmpty(scopeName))
            return new ScopeServiceResult<bool>(ScopeErrors.ScopeInvalidData,
                new ServiceError("ERR_MISSING_DATA", $"You must provide a non-blank scope name."));
        
        try
        {
            // Verify if tenant exists
            var matching = await _scopeRepository
                .FirstOrDefault(s => s.ParentTenantId == tenantId&& s.Name == scopeName);
            if (matching == null)
            {
                return new ScopeServiceResult<bool>(ScopeErrors.ScopeNotFound,
                    new ServiceError("ERR_SCOPE_NOT_FOUND", $"No such scope with name {scopeName} exists."));
            }
            
            // Delete the matching tenant
            await _scopeRepository.DeleteEntity(matching);
            return new ScopeServiceResult<bool>(true);
        }
        catch (Exception e)
        {
            // Handle DB errors (return 500)
            Console.WriteLine($"Service fail");
            Console.WriteLine($"\tMessage: {e.Message}");
            Console.WriteLine($"\tStack Frame: {e.StackTrace}");
            
            return new ScopeServiceResult<bool>(ScopeErrors.ServerError, 
                new ServiceError("INTERNAL_ERROR", "An unknown error occured while trying to create a new scope."));
        }
    }
}