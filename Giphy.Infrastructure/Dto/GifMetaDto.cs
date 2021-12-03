using System.Net;

namespace Giphy.Infrastructure.Dto;

public class GifMetaDto
{
    public HttpStatusCode Status { get; set; }
    public string? Msg { get; set; }
}