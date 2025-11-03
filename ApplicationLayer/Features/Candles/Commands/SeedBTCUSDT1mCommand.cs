using ApplicationLayer.Dto.Candles;
using MediatR;

namespace ApplicationLayer.Features.Candles.Commands;

public record SeedBTCUSDT1mCommand(SeedCandlesRequestDto Model) : IRequest<HandlerResult>;