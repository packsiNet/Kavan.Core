using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Portfolio.Commands;
using ApplicationLayer.Interfaces.Portfolio;
using MediatR;

namespace ApplicationLayer.Features.Portfolio.Handler;

public class DeletePortfolioSymbolHandler(IPortfolioService _service) : IRequestHandler<DeletePortfolioSymbolCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeletePortfolioSymbolCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteSymbolAsync(request.Symbol);
        return result.ToHandlerResult();
    }
}

