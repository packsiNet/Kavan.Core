using ApplicationLayer.Dto.BaseDtos;
using MediatR;

namespace ApplicationLayer.Features.Candles.Query;

public record GetTimeframesQuery : IRequest<HandlerResult>;
