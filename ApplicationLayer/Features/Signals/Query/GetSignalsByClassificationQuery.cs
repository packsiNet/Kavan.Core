using ApplicationLayer;
using MediatR;

namespace ApplicationLayer.Features.Signals.Query
{
    public record GetSignalsByClassificationQuery(string Category, string Name) : IRequest<HandlerResult>;
}