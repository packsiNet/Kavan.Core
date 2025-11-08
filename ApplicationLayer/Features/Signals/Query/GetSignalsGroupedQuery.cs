using ApplicationLayer;
using MediatR;

namespace ApplicationLayer.Features.Signals.Query
{
    public record GetSignalsGroupedQuery() : IRequest<HandlerResult>;
}