using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Abstract;
using Trustedbits.ApiServer.Services.Interfaces;

namespace Trustedbits.ApiServer.Services;

public class ScopeService : ScopeServiceBase
{
    private const string ScopeRegex = "^[A-Za-z]+:[A-Za-z]+$";
    private readonly IRepository<Scope> _scopeRepository;
    private readonly IMapper _objectMapper;

    public ScopeService(IRepository<Scope> scopeRepository, IMapper objectMapper)
    {
        _scopeRepository = scopeRepository;
        _objectMapper = objectMapper;
    }

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

    public override async Task<ScopeServiceResult<List<ScopeDto>>> GetAllScopes(Guid tenantId, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public override async Task<ScopeServiceResult<List<ScopeDto>>> GetScope(Guid tenantId, ScopeQueryDto scopeQueryData)
    {
        throw new NotImplementedException();
    }
    
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

    public override async Task<ScopeServiceResult<bool>> DeleteScope(Guid tenantId, string scopeName)
    {
        throw new NotImplementedException();
    }
}