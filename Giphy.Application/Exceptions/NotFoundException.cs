using System.Net;

namespace Giphy.Application.Exceptions;

public class NotFoundException : StatusCodeException
{
    public NotFoundException(
        string message = "The server cannot process the request due to client error/s."
    ) : base(message, (int)HttpStatusCode.NotFound)
    {
    }
}