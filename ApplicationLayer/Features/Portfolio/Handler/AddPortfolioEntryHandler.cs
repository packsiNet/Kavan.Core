using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Portfolio.Commands;
using ApplicationLayer.Interfaces.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Handler;

public class AddPortfolioEntryHandler(IPortfolioService _service) : IRequestHandler<AddPortfolioEntryCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(AddPortfolioEntryCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.AddEntryAsync(request.Model);
        return result.ToHandlerResult();
    }
}