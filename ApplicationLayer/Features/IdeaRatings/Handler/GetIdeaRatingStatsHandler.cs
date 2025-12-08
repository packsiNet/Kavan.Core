using ApplicationLayer.Extensions;
using ApplicationLayer.Features.IdeaRatings.Query;
using ApplicationLayer.Interfaces.IdeaRatings;
using MediatR;

namespace ApplicationLayer.Features.IdeaRatings.Handler;

public class GetIdeaRatingStatsHandler(IIdeaRatingService _service) : IRequestHandler<GetIdeaRatingStatsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetIdeaRatingStatsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetRatingStatsAsync(request.IdeaId);
        return result.ToHandlerResult();
    }
}
