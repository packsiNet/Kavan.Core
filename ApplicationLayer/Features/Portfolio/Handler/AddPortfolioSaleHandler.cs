using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Portfolio.Commands;
using ApplicationLayer.Interfaces.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Handler;

public class AddPortfolioSaleHandler(IPortfolioService _service) : IRequestHandler<AddPortfolioSaleCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(AddPortfolioSaleCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.AddSaleAsync(request.Model);
        return result.ToHandlerResult();
    }
}

