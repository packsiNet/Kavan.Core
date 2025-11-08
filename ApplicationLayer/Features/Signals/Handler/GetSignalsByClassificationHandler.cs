using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Signals.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Signals.Handler
{
    public class GetSignalsByClassificationHandler(IRepository<Signal> _signals)
        : IRequestHandler<GetSignalsByClassificationQuery, HandlerResult>
    {
        public async Task<HandlerResult> Handle(GetSignalsByClassificationQuery request, CancellationToken cancellationToken)
        {
            var list = await _signals.Query()
                .Where(x => x.SignalCategory == request.Category && x.SignalName == request.Name)
                .OrderByDescending(x => x.SignalTime)
                .Select(x => new SignalSummaryDto
                {
                    Id = x.Id,
                    Symbol = x.Symbol,
                    Timeframe = x.Timeframe,
                    SignalTime = x.SignalTime,
                    SignalName = x.SignalName,
                    Direction = x.Direction,
                    BreakoutLevel = x.BreakoutLevel
                })
                .ToListAsync(cancellationToken);

            return Result<List<SignalSummaryDto>>.Success(list).ToHandlerResult();
        }
    }
}