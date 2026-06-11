namespace Trustedbits.ApiServer.Core.Dto;

public class ErrorDto
{
    public string Message { get; set; }
    public object? Detail { get; set; }

    public ErrorDto(string message, object? detail = null)
    {
        Message = message;
        Detail = detail;
    }
}