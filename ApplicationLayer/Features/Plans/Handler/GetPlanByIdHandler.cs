using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Plans.Query;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Plans.Handler;

public class GetPlanByIdHandler(IPlanService _planService)
    : IRequestHandler<GetPlanByIdQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _planService.GetByIdAsync(request.Id);
        return result.ToHandlerResult();
    }
}