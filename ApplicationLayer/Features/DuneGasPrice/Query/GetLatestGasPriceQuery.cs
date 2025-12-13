using DomainLayer.Entities;
using MediatR;

namespace ApplicationLayer.Features.DuneGasPrice.Query;

public record GetLatestGasPriceQuery() : IRequest<List<DuneGasPriceSnapshot>>;
