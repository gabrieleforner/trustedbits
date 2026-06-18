namespace Trustedbits.ApiServer.Core.Dto;

/// <summary>
/// DTO for describe service-level errors.
/// </summary>
public class ErrorDto
{
    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Object containing error details.
    /// </summary>
    public object? Detail { get; set; }

    /// <summary>
    /// Default constructor for error DTO.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="detail">Error details.</param>
    public ErrorDto(string message, object? detail = null)
    {
        Message = message;
        Detail = detail;
    }
}