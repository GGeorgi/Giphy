using Giphy.Application.Interfaces;
using Giphy.Domain.Entities;

namespace Giphy.Infrastructure.Dto;

public class GifImageDto : IAutoMap<Gif, GifImageDto>
{
    public string Type { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Title { get; set; } = null!;
}