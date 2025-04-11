namespace Core.Exceptions;

public class ExternalErrorException : Exception
{
    public string ErrorMessage { get; }

    public ExternalErrorException(string serviceName, string errorMessage) : base($"External service {serviceName} throw an exception.")
    {
        ErrorMessage = errorMessage;
    }
}