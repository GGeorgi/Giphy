using Giphy.Domain.Entities;

namespace Giphy.Application.Interfaces.Repositories;

public interface IGifRepository
{
    Task<IEnumerable<Gif>?> GetAsync(string query);
    Task AddAsync(string query, IEnumerable<Gif> gifs);
}