namespace Core.Exceptions;

public class ValidationErrorException : Exception
{
    public IDictionary<string, string[]> Errors { get; set; }

    public ValidationErrorException(IDictionary<string, string[]> errors) : base("One or more validation errors occured.")
    {
        Errors = errors;
    }
}