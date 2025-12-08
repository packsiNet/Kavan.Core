using ApplicationLayer.Dto.IdeaRatings;
using MediatR;

namespace ApplicationLayer.Features.IdeaRatings.Commands;

public record AddIdeaRatingCommand(AddIdeaRatingDto Model) : IRequest<HandlerResult>;
