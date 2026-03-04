namespace Trustedbits.ApiServer.Models;

public class ServiceError
{
    public string ErrorString;
    public string ErrorMessage;
    
    public ServiceError(string errorString, string errorMessage)
    {
        ErrorString = errorString;
        ErrorMessage = errorMessage;
    }
}

public class ServiceResult<TErrorEnum, TSuccess> where TErrorEnum : struct
{
    public bool IsFailed { get; }
    public bool IsSuccess => !IsFailed;
    
    public TSuccess? Success { get; }
    public TErrorEnum? Error { get; }
    public ServiceError? ErrorData { get; }
    
    
    // Success constructor
    public ServiceResult(TSuccess success)
    {
        IsFailed = false;
        Success = success;
    }
    
    // Failure constructor
    public ServiceResult(TErrorEnum error, ServiceError errorData)
    {
        IsFailed = true;
        Error = error;
        ErrorData = errorData;
    }
}