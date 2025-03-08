namespace Printpress.Application;

public class ValidationExeption : Exception
{
    public ValidationExeption(string message) : base(message)
    {

    }


    public static void FireValidationException(string message, params object?[] args)
    {
        string formatedMessage = string.Format(message, args);
        throw new ValidationExeption(formatedMessage);
    }
}
