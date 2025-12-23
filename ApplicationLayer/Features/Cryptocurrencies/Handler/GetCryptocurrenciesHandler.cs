using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Cryptocurrency;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Cryptocurrencies.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Cryptocurrencies.Handler;

public class GetCryptocurrenciesHandler(IRepository<Cryptocurrency> _repository) 
    : IRequestHandler<GetCryptocurrenciesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetCryptocurrenciesQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.Query()
            .AsNoTracking()
            .OrderBy(c => c.Symbol)
            .Select(c => new CryptocurrencyDto(c.Id, c.Symbol, c.Name))
            .ToListAsync(cancellationToken);

        return Result<List<CryptocurrencyDto>>.Success(list).ToHandlerResult();
    }
}
