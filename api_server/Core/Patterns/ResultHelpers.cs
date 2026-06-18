using Trustedbits.ApiServer.Core.Dto;

namespace Trustedbits.ApiServer.Core.Patterns;

/// <summary>
/// Helper functions specializations for scope service.
/// </summary>
/// <typeparam name="T">Data type to return.</typeparam>
public static class ResultHelpers<T>
{
    /// <summary>
    /// Helper for returning a NotFoundError
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Result<T> NotFoundError(Guid id)
    {
        var errorDict = new Dictionary<string, object>
        {
            { "ProvidedId", id }
        };
        var errorDto = new ErrorDto("Resource not found", errorDict);
        return new Result<T>(errorDto, ErrorType.NotFound);
    }
    
    /// <summary>
    /// Helper for returning a data conflict error.
    /// </summary>
    /// <param name="conflictAttrib">Name of the attribute who is originating the conflict.</param>
    /// <param name="conflictValue">the actual conflicting value.</param>
    /// <returns>Result with ErrorType set as conflict.</returns>
    public static Result<T> ConflictError(string conflictAttrib, string conflictValue)
    {
        var conflictDict = new Dictionary<string, string>
        {
            { "ConflictingAttribute", conflictAttrib },
            { "ConflictingValue", conflictValue }
        };
        
        var errorDto = new ErrorDto("Conflicting value found", conflictDict);
        return new Result<T>(errorDto, ErrorType.Conflict);
    }
    
    /// <summary>
    /// Helper to return a invalid ID error (Specialization of <see cref="BadRequest"/>).
    /// </summary>
    /// <returns>Result with ErrorType set as BadRequest.</returns>
    public static Result<T> InvalidIdError()
    {
        return BadRequest("Provided ID is not valid");
    }

    /// <summary>
    /// Helper for validating paging values (specialization of <see cref="BadRequest"/>).
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of elements per page</param>
    /// <returns>null, or error with ErrorType as BadRequest</returns>
    public static Result<T>? ValidatePagingSettings(int page, int pageSize)
    {
        if (page < 1)
            return BadRequest("Invalid page number");
        if (pageSize < 1)
            return BadRequest("Invalid page size");
        return null;
    }

    /// <summary>
    /// Helper for returning a bad request error.
    /// </summary>
    /// <param name="error">String containing the error message.</param>
    /// <param name="detail">Generic object containing details.</param>
    /// <returns></returns>
    public static Result<T> BadRequest(string error, object? detail= null)
    {
        var errorDto = new ErrorDto(error, detail);
        return new Result<T>(errorDto, ErrorType.BadRequest);
    }
}