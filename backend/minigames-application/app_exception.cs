using System.Net;

namespace Minigames.Application;

public class AppException : Exception
{
    public HttpStatusCode Code { get; }

    public AppException(string message) : base(message)
    {
        Code = HttpStatusCode.BadRequest;
    }

    public AppException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AppException(string message, HttpStatusCode code) : base(message)
    {
        Code = code;
    }
}

