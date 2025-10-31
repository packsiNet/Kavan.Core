using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Plans.Query;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Plans.Handler;

public class GetPlansHandler(IPlanService _planService)
    : IRequestHandler<GetPlansQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var result = await _planService.GetAllAsync();
        return result.ToHandlerResult();
    }
}