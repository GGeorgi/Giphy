using System.Net;
using System.Net.Http.Json;
using AutoMapper;
using Giphy.Application.Exceptions;
using Giphy.Application.Filters;
using Giphy.Application.Interfaces.Services;
using Giphy.Domain.Entities;
using Giphy.Infrastructure.Configs;
using Giphy.Infrastructure.Dto;
using Microsoft.Extensions.Options;

namespace Giphy.Infrastructure.Services;

public class GifService : IGifService
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly GiphyOptions _giphyOptions;

    public GifService(IOptions<GiphyOptions> giOptions, HttpClient httpClient, IMapper mapper)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _giphyOptions = giOptions.Value;
    }

    public async Task<IEnumerable<Gif>> Search(GifFilter filter)
    {
        var url = $"{_giphyOptions.GifUrl}?api_key={_giphyOptions.ApiKey}&{filter.Query}";
        var result = await _httpClient.GetFromJsonAsync<GifResponseDto>(url);

        if (result == null) throw new BadRequestException();
        if (result.Meta.Status != HttpStatusCode.OK)
            throw new BadRequestException(result.Meta.Msg ?? $"Operation failed with {result.Meta.Status} code");
        if (result.Data == null) return Array.Empty<Gif>();

        return _mapper.Map<List<Gif>>(result.Data);
    }
}