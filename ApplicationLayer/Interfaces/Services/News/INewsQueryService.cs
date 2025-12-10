using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.News;

namespace ApplicationLayer.Interfaces.Services.News;

public interface INewsQueryService
{
    Task<Result<NewsPageDto>> GetAsync(GetNewsRequestDto dto);
}
