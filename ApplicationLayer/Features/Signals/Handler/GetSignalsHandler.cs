using ApplicationLayer.Common;
using ApplicationLayer.Common.Enums;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Features.Signals.Query;
using ApplicationLayer.Interfaces.Signals;
using MediatR;

namespace ApplicationLayer.Features.Signals.Handler;

public class GetSignalsHandler(ISignalService _signalService) : IRequestHandler<GetSignalsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetSignalsQuery request, CancellationToken cancellationToken)
    {
        var list = await _signalService.GetSignalsAsync(request.Symbol, request.TimeFrame, request.Limit, cancellationToken);

        return new HandlerResult
        {
            RequestStatus = RequestStatus.Successful,
            ObjectResult = list,
            Message = CommonMessages.Successful
        };
    }
}