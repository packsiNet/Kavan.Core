using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Plans.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Plans.Handler;

public class DeletePlanHandler(IPlanService _planService)
    : IRequestHandler<DeletePlanCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var result = await _planService.DeleteAsync(request.Id);
        return result.ToHandlerResult();
    }
}