namespace Core.Exceptions;

public class ValidationErrorException(IDictionary<string, string[]> errors) : Exception("One or more validation errors occured.")
{
    public IDictionary<string, string[]> Errors { get; set; } = errors;
}