using ApplicationLayer.Extensions;
using ApplicationLayer.Interfaces;
using MediatR;

namespace ApplicationLayer.Features.Candles.Handler;

public class SeedBTCUSDT1mHandler(ICandleSeedService seedService) : IRequestHandler<ApplicationLayer.Features.Candles.Commands.SeedBTCUSDT1mCommand, HandlerResult>
{
    private readonly ICandleSeedService _seedService = seedService;

    public async Task<HandlerResult> Handle(Commands.SeedBTCUSDT1mCommand request, CancellationToken cancellationToken)
    {
        var count = request.Model?.Count ?? 100;
        var startTime = request.Model?.StartTime;

        var result = await ResultExtensions.ExecuteAsync(async () =>
        {
            var inserted = await _seedService.SeedBTCUSDT_1m_DoubleTopBreakoutAsync(count, startTime, cancellationToken);
            return inserted;
        });

        return result.ToHandlerResult();
    }
}