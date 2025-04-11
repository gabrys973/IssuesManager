namespace Core.Exceptions;

public class TokenEmptyException(string serviceName) : Exception($"Token for {serviceName} service cannot be empty or null.")
{
}