namespace Giphy.Infrastructure.Dto;

public class GifResponseDto
{
    public List<GifImageDto>? Data { get; set; }
    public GifMetaDto Meta { get; set; } = null!;
}