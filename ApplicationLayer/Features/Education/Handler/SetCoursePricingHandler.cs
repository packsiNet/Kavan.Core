using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class SetCoursePricingHandler(ICourseService _service) : IRequestHandler<SetCoursePricingCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(SetCoursePricingCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.SetPricingAsync(request.Id, request.Model.Price, request.Model.IsFree);
        return result.ToHandlerResult();
    }
}