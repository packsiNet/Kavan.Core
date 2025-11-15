using ApplicationLayer.Dto.Signals;
using MediatR;

namespace ApplicationLayer.Features.Signals.Query
{
    public record GetSignalsQuery(SignalRequestDto Model) : IRequest<HandlerResult>;
}