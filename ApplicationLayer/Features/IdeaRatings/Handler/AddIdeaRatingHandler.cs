using ApplicationLayer.Extensions;
using ApplicationLayer.Features.IdeaRatings.Commands;
using ApplicationLayer.Interfaces.IdeaRatings;
using MediatR;

namespace ApplicationLayer.Features.IdeaRatings.Handler;

public class AddIdeaRatingHandler(IIdeaRatingService _service) : IRequestHandler<AddIdeaRatingCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(AddIdeaRatingCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.AddRatingAsync(request.Model);
        return result.ToHandlerResult();
    }
}
