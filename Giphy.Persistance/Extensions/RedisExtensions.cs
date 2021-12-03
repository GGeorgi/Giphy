using Giphy.Persistance.Helpers;
using StackExchange.Redis;

namespace Giphy.Persistance.Extensions;

public static class RedisExtensions
{
    public static async Task<IEnumerable<T>> MGetAsync<T>(this IDatabaseAsync db, IEnumerable<RedisValue> keys)
    {
        var data = await db.StringGetAsync(keys.Select(x => (RedisKey)x.ToString()).ToArray());
        return data.Select(x => ProtoSerializer.Deserialize<T>(x)).Where(x => x != null)!;
    }
}