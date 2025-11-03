using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Plans.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Plans.Handler;

public class UpdatePlanHandler(IPlanService _planService)
    : IRequestHandler<UpdatePlanCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        var result = await _planService.UpdateAsync(request.Id, request.Model);
        return result.ToHandlerResult();
    }
}