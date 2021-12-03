using Giphy.Application.Interfaces.Repositories;
using Giphy.Domain.Entities;
using Giphy.Persistance.Extensions;
using Giphy.Persistance.Helpers;
using StackExchange.Redis;

namespace Giphy.Persistance.Repositories;

public class GifRepository : IGifRepository
{
    private readonly IDatabaseAsync _database;

    public GifRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<IEnumerable<Gif>?> GetAsync(string query)
    {
        var oldIds = await _database.SetMembersAsync(query);
        if (!oldIds.Any()) return null;

        return await _database.MGetAsync<Gif>(oldIds);
    }

    public Task AddAsync(string query, IEnumerable<Gif> gifs)
    {
        var pipe = new PipelineManager();

        foreach (var gif in gifs)
        {
            pipe.AddTask(_database.SetAddAsync(query, gif.Id));
            pipe.AddTask(_database.StringSetAsync(gif.Id, ProtoSerializer.Serialize(gif)));
        }

        return pipe.CommitAsync();
    }
}