using MediatR;

namespace ApplicationLayer.Features.IdeaRatings.Query;

public record GetIdeaRatingStatsQuery(int IdeaId) : IRequest<HandlerResult>;
