using ApplicationLayer.Dto.BaseDtos;
using MediatR;

namespace ApplicationLayer.Features.Candles.Query;

public record GetCandlesQuery(int CryptocurrencyId, string Timeframe, int Limit = 100) : IRequest<HandlerResult>;
