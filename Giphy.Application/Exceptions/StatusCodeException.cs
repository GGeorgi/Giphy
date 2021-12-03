using System;
using System.Net;

namespace Giphy.Application.Exceptions;

public class StatusCodeException : Exception
{
    public int StatusCode { get; }

    public StatusCodeException(
        string message = null!,
        int statusCode = (int)HttpStatusCode.BadRequest
    ) : this(message, null, statusCode)
    {
    }

    public StatusCodeException(
        string message,
        Exception? innerException = null,
        int statusCode = (int)HttpStatusCode.BadRequest) : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}