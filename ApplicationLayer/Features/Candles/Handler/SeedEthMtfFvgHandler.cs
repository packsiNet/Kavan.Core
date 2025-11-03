using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Candles.Commands;
using ApplicationLayer.Interfaces;
using MediatR;

namespace ApplicationLayer.Features.Candles.Handler
{
    public class SeedEthMtfFvgHandler : IRequestHandler<SeedEthMtfFvgCommand, HandlerResult>
    {
        private readonly ICandleSeedService _seedService;

        public SeedEthMtfFvgHandler(ICandleSeedService seedService)
        {
            _seedService = seedService;
        }

        public async Task<HandlerResult> Handle(SeedEthMtfFvgCommand request, CancellationToken cancellationToken)
        {
            var inserted = await _seedService.SeedETHUSDT_MTF_FVG_StructureAsync(
                request.Days,
                request.M5Bars,
                request.M1Bars,
                request.StartTime,
                cancellationToken);

            return Result<int>.Success(inserted).ToHandlerResult();
        }
    }
}