using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Portfolio.Query;
using ApplicationLayer.Interfaces.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Handler;

public class GetPortfolioEntriesBySymbolHandler(IPortfolioService _service) : IRequestHandler<GetPortfolioEntriesBySymbolQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetPortfolioEntriesBySymbolQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetEntriesBySymbolAsync(request.Symbol, request.Model);
        return result.ToHandlerResult();
    }
}