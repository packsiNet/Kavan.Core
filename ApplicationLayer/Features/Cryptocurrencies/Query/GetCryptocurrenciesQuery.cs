using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Cryptocurrency;
using MediatR;

namespace ApplicationLayer.Features.Cryptocurrencies.Query;

public record GetCryptocurrenciesQuery : IRequest<HandlerResult>;
