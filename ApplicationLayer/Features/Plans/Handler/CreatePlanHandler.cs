using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Plans.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Plans.Handler;

public class CreatePlanHandler(IPlanService _planService)
    : IRequestHandler<CreatePlanCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var result = await _planService.CreateAsync(request.Model);
        return result.ToHandlerResult();
    }
}