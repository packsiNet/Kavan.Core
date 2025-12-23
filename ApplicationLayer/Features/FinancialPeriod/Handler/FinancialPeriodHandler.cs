using ApplicationLayer.Dto.FinancialPeriod;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.FinancialPeriod.Commands;
using ApplicationLayer.Features.FinancialPeriod.Query;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.FinancialPeriod.Handler;

public class FinancialPeriodHandler(IFinancialPeriodService _service) :
    IRequestHandler<CreateFinancialPeriodCommand, HandlerResult>,
    IRequestHandler<CloseFinancialPeriodCommand, HandlerResult>,
    IRequestHandler<GetActiveFinancialPeriodQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateFinancialPeriodCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request.Model);
        return result.ToHandlerResult<FinancialPeriodDto>();
    }

    public async Task<HandlerResult> Handle(CloseFinancialPeriodCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.ClosePeriodAsync(request.Id);
        return result.ToHandlerResult<FinancialPeriodDto>();
    }

    public async Task<HandlerResult> Handle(GetActiveFinancialPeriodQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.GetActivePeriodAsync();
        return result.ToHandlerResult<FinancialPeriodDto>();
    }
}
