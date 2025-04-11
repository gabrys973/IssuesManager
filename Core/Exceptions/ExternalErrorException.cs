namespace Core.Exceptions;

public class ExternalErrorException(string serviceName, string errorMessage) : Exception($"External {serviceName} service threw an exception.")
{
    public string ErrorMessage { get; } = errorMessage;
    public string ServiceName { get; } = serviceName;
}