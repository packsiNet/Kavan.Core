using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Portfolio.Query;
using ApplicationLayer.Interfaces.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Handler;

public class GetPortfolioPositionsHandler(IPortfolioService _service) : IRequestHandler<GetPortfolioPositionsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetPortfolioPositionsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetPositionsAsync(request.Model);
        return result.ToHandlerResult();
    }
}