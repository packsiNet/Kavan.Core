using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Signals.Query;
using ApplicationLayer.Interfaces.Services.Signals;
using MediatR;

namespace ApplicationLayer.Features.Signals.Handler
{
    public class GetSignalsHandler(ISignalAnalysisService _signalService)
        : IRequestHandler<GetSignalsQuery, HandlerResult>
    {
        public async Task<HandlerResult> Handle(GetSignalsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var list = await _signalService.AnalyzeAsync(request.Model, cancellationToken);
                return Result<IReadOnlyList<SignalResultDto>>.Success(list).ToHandlerResult();
            }
            catch (Exception ex)
            {
                return Result.GeneralFailure($"Signal analysis failed: {ex.Message}").ToHandlerResult();
            }
        }
    }
}