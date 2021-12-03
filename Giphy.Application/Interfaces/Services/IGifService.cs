using Giphy.Application.Filters;
using Giphy.Domain.Entities;

namespace Giphy.Application.Interfaces.Services;

public interface IGifService
{
    Task<IEnumerable<Gif>> Search(GifFilter filter);
}