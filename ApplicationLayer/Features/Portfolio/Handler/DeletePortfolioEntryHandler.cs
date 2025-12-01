using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Portfolio.Commands;
using ApplicationLayer.Interfaces.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Handler;

public class DeletePortfolioEntryHandler(IPortfolioService _service) : IRequestHandler<DeletePortfolioEntryCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeletePortfolioEntryCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteEntryAsync(request.Id);
        return result.ToHandlerResult();
    }
}