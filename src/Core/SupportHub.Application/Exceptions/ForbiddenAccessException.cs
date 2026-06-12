namespace SupportHub.Application.Exceptions;

public sealed class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "Access forbidden") : base(message)
    {
        
    }
}