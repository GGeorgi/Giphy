using MediatR;

namespace Giphy.Interfaces
{
    public interface IUseCase<TInputModel, TOutputModel> : IRequest<TOutputModel>
    {
        TInputModel Model { get; }
    }
}