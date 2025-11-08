using ApplicationLayer;
using MediatR;

namespace ApplicationLayer.Features.Signals.Query
{
    public record GetSignalByIdQuery(int Id) : IRequest<HandlerResult>;
}