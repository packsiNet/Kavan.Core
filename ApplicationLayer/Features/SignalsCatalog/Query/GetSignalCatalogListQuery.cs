using ApplicationLayer.Dto.SignalsCatalog;
using MediatR;

namespace ApplicationLayer.Features.SignalsCatalog.Query
{
    public record GetSignalCatalogListQuery() : IRequest<List<SignalCatalogListItemDto>>;
}