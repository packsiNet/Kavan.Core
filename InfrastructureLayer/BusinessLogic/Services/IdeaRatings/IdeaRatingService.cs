using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.IdeaRatings;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.IdeaRatings;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.IdeaRatings;

[InjectAsScoped]
public class IdeaRatingService(
    IUnitOfWork _uow,
    IRepository<IdeaRating> _ratings,
    IRepository<Idea> _ideas,
    IUserContextService _userContext) : IIdeaRatingService
{
    public async Task<Result<IdeaRatingStatsDto>> AddRatingAsync(AddIdeaRatingDto dto)
    {
        if (_userContext.UserId == null)
            return Result<IdeaRatingStatsDto>.AuthenticationFailure();

        var idea = await _ideas.GetByIdAsync(dto.IdeaId);
        if (idea == null)
            return Result<IdeaRatingStatsDto>.NotFound("ایده یافت نشد");

        if (dto.Rating < 1 || dto.Rating > 5)
            return Result<IdeaRatingStatsDto>.ValidationFailure("امتیاز باید بین 1 تا 5 باشد");

        var userId = _userContext.UserId.Value;
        var existing = await _ratings.GetDbSet()
            .FirstOrDefaultAsync(x => x.IdeaId == dto.IdeaId && x.UserId == userId);

        await _uow.BeginTransactionAsync();
        if (existing != null)
        {
            existing.Rating = dto.Rating;
            existing.MarkAsUpdated();
            await _ratings.UpdateAsync(existing);
        }
        else
        {
            var entity = new IdeaRating
            {
                IdeaId = dto.IdeaId,
                UserId = userId,
                Rating = dto.Rating
            };
            await _ratings.AddAsync(entity);
        }
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return await GetRatingStatsAsync(dto.IdeaId);
    }

    public async Task<Result<IdeaRatingStatsDto>> GetRatingStatsAsync(int ideaId)
    {
        var ratings = await _ratings.GetDbSet()
            .Where(x => x.IdeaId == ideaId)
            .ToListAsync();

        var count = ratings.Count;
        var average = count > 0 ? ratings.Average(x => x.Rating) : 0;

        int? myRating = null;
        if (_userContext.UserId != null)
        {
            var my = ratings.FirstOrDefault(x => x.UserId == _userContext.UserId);
            if (my != null)
                myRating = my.Rating;
        }

        return Result<IdeaRatingStatsDto>.Success(new IdeaRatingStatsDto
        {
            IdeaId = ideaId,
            AverageRating = average,
            TotalRatings = count,
            MyRating = myRating
        });
    }
}
