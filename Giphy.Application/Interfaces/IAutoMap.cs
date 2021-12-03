using AutoMapper;

namespace Giphy.Application.Interfaces
{
    public interface IAutoMap<TEntity, TDto>
        where TEntity : class, new()
        where TDto : class, new()
    {
        void CreateMap(Profile profile)
        {
            profile.CreateMap<TEntity, TDto>().ReverseMap();
        }
    }
}