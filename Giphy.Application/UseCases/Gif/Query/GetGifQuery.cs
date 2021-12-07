using Giphy.Application.Exceptions;
using Giphy.Application.Filters;
using Giphy.Application.Interfaces.Repositories;
using Giphy.Application.Interfaces.Services;
using Giphy.Interfaces;
using MediatR;
using RedLockNet.SERedis;

namespace Giphy.Application.UseCases.Gif.Query;

public class GetGifQuery : IUseCase<GifFilter, IEnumerable<Domain.Entities.Gif>>
{
    public GifFilter Model { get; init; } = null!;

    public class Handler : IRequestHandler<GetGifQuery, IEnumerable<Domain.Entities.Gif>>
    {
        private readonly IGifService _gifService;
        private readonly RedLockFactory _lockFactory;
        private readonly IGifRepository _repository;

        public Handler(IGifRepository repository, IGifService gifService, RedLockFactory lockFactory)
        {
            _repository = repository;
            _gifService = gifService;
            _lockFactory = lockFactory;
        }

        public async Task<IEnumerable<Domain.Entities.Gif>> Handle(GetGifQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var oldResult = await _repository.GetAsync(model.Query);
            if (oldResult != null) return oldResult;

            var resource = model.Query;
            var expiry = TimeSpan.FromSeconds(5);
            var wait = TimeSpan.FromSeconds(5);
            var retry = TimeSpan.FromSeconds(1);
            await using var redLock = await _lockFactory.CreateLockAsync(resource, expiry, wait, retry);
            // make sure we got the lock
            if (!redLock.IsAcquired) throw new BadRequestException("Timeout exception. Try again please");

            var doubleCheckResult = await _repository.GetAsync(model.Query);
            if (doubleCheckResult != null) return doubleCheckResult;

            var result = await _gifService.Search(model);
            await _repository.AddAsync(model.Query, result);
            return result;
        }
    }
}