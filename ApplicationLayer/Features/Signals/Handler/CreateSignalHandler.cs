using ApplicationLayer.Common;
using ApplicationLayer.Common.Enums;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Features.Signals.Commands;
using ApplicationLayer.Interfaces.Signals;
using MediatR;

namespace ApplicationLayer.Features.Signals.Handler;

public class CreateSignalHandler(ISignalService _signalService) : IRequestHandler<CreateSignalCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateSignalCommand request, CancellationToken cancellationToken)
    {
        var list = await _signalService.GenerateSignalsAsync(request.Symbol, cancellationToken);

        return new HandlerResult
        {
            RequestStatus = RequestStatus.Successful,
            ObjectResult = list,
            Message = list.Count > 0 ? CommonMessages.Successful : "No signal generated"
        };
    }
}