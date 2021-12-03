using Giphy.Application.Filters;
using Giphy.Application.Interfaces.Repositories;
using Giphy.Application.Interfaces.Services;
using Giphy.Application.Threading;
using Giphy.Interfaces;
using MediatR;

namespace Giphy.Application.UseCases.Gif.Query;

public class GetGifQuery : IUseCase<GifFilter, IEnumerable<Domain.Entities.Gif>>
{
    public GifFilter Model { get; init; } = null!;

    public class Handler : IRequestHandler<GetGifQuery, IEnumerable<Domain.Entities.Gif>>
    {
        private readonly IGifService _gifService;
        private readonly ConsistentHash<TaskQueue> _lockPool;
        private readonly IGifRepository _repository;

        public Handler(IGifRepository repository, IGifService gifService, ConsistentHash<TaskQueue> lockPool)
        {
            _repository = repository;
            _gifService = gifService;
            _lockPool = lockPool;
        }

        public async Task<IEnumerable<Domain.Entities.Gif>> Handle(GetGifQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var oldResult = await _repository.GetAsync(model.Query);
            if (oldResult != null) return oldResult;

            var q = _lockPool.GetNode(request.Model.Query);
            return await q.Enqueue(async () =>
            {
                var doubleCheckResult = await _repository.GetAsync(model.Query);
                if (doubleCheckResult != null) return doubleCheckResult;

                var result = await _gifService.Search(model);
                await _repository.AddAsync(model.Query, result);
                return result;
            });
        }
    }
}