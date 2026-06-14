using Trustedbits.ApiServer.Core.Contracts;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;

namespace Trustedbits.ApiServer.Core;

public static class ScopeHelpers<T>
{
    public static ScopeServiceResult<T> NotFoundError(Guid id)
    {
        var errorDict = new Dictionary<string, object>
        {
            { "ScopeId", id }
        };
        var errorDto = new ErrorDto("Scope not found", errorDict);
        return new ScopeServiceResult<T>(errorDto, ErrorType.NotFound);
    }
    
    public static ScopeServiceResult<T> ConflictError(string conflictAttrib, string conflictValue)
    {
        var conflictDict = new Dictionary<string, string>
        {
            { "ConflictingAttribute", conflictAttrib },
            { "ConflictingValue", conflictValue }
        };
        
        var errorDto = new ErrorDto("Conflicting value found", conflictDict);
        return new ScopeServiceResult<T>(errorDto, ErrorType.Conflict);
    }
    
    public static ScopeServiceResult<T> InvalidScopeIdError()
    {
        var errorDto = new ErrorDto("Scope not found");
        return new ScopeServiceResult<T>(errorDto, ErrorType.NotFound);
    }

    public static ScopeServiceResult<T>? ValidatePagingSettings(int page, int pageSize)
    {
        if (page < 1)
        {
            var errorDto = new ErrorDto("Invalid page number");
            return new ScopeServiceResult<T>(errorDto, ErrorType.BadRequest);
        }

        if (pageSize < 1)
        {
            var errorDto = new ErrorDto("Invalid page size");
            return new ScopeServiceResult<T>(errorDto, ErrorType.BadRequest);
        }

        return null;
    }
}