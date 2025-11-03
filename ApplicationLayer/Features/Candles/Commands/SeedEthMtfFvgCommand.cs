using System;
using MediatR;
using ApplicationLayer;

namespace ApplicationLayer.Features.Candles.Commands
{
    public record SeedEthMtfFvgCommand(
        int Days = 12,
        int M5Bars = 60,
        int M1Bars = 120,
        DateTime? StartTime = null
    ) : IRequest<HandlerResult>;
}