using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.IdeaRatings;

namespace ApplicationLayer.Interfaces.IdeaRatings;

public interface IIdeaRatingService
{
    Task<Result<IdeaRatingStatsDto>> AddRatingAsync(AddIdeaRatingDto dto);
    Task<Result<IdeaRatingStatsDto>> GetRatingStatsAsync(int ideaId);
}
