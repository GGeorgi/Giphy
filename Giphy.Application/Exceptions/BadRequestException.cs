using System.Net;

namespace Giphy.Application.Exceptions;

public class BadRequestException : StatusCodeException
{
    public BadRequestException(
        string message = "The server cannot process the request due to client error/s."
    ) : base(message, (int)HttpStatusCode.BadRequest)
    {
    }
}