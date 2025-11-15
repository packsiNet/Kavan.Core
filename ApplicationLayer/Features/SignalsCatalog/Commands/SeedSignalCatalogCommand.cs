using MediatR;

namespace ApplicationLayer.Features.SignalsCatalog.Commands
{
    public record SeedSignalCatalogCommand() : IRequest<bool>;
}