namespace Marventa.Framework.Middleware;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message)
    {
    }
}
