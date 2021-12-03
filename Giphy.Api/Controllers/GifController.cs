using Giphy.Application.Filters;
using Giphy.Application.UseCases.Gif.Query;
using Giphy.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Giphy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GifController : ControllerBase
{
    private readonly IMediator _mediator;

    public GifController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<IEnumerable<Gif>> Search([FromQuery] GifFilter filter)
    {
        return _mediator.Send(new GetGifQuery { Model = filter });
    }
}