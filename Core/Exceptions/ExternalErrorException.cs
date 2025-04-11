namespace Core.Exceptions;

public class ExternalErrorException : Exception
{
    public string ErrorMessage { get; }
    public string ServiceName { get; }

    public ExternalErrorException(string serviceName, string errorMessage) : base($"External {serviceName} service threw an exception.")
    {
        ServiceName = serviceName;
        ErrorMessage = errorMessage;
    }
}