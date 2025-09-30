namespace Marventa.Framework.ExceptionHandling;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message)
    {
    }
}
