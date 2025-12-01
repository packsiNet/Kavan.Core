using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Portfolio.Commands;
using ApplicationLayer.Interfaces.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Handler;

public class UpdatePortfolioEntryHandler(IPortfolioService _service) : IRequestHandler<UpdatePortfolioEntryCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdatePortfolioEntryCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateEntryAsync(request.Id, request.Model);
        return result.ToHandlerResult();
    }
}