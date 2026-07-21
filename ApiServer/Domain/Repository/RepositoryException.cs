namespace Trustedbits.ApiServer.Domain.Repository;

public enum RepositoryExceptionType
{
    UniqueValueViolation,
    NonNullableValueException,
    ServerException
}

public class RepositoryException : Exception
{
    public RepositoryExceptionType Type { get; init; }

    public RepositoryException(string? message, RepositoryExceptionType type) : base(message)
    {
        Type = type;
    }
}