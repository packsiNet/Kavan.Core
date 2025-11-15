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
    public class GetSignalsGroupedHandler(IRepository<Signal> _signals)
        : IRequestHandler<GetSignalsGroupedQuery, HandlerResult>
    {
        public async Task<HandlerResult> Handle(GetSignalsGroupedQuery request, CancellationToken cancellationToken)
        {
            var flat = await _signals.Query()
                .GroupBy(x => new { x.SignalCategory, x.SignalName })
                .Select(g => new
                {
                    Category = g.Key.SignalCategory,
                    Name = g.Key.SignalName,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var grouped = flat
                .GroupBy(x => x.Category)
                .Select(g => new SignalCategoryDto
                {
                    Category = g.Key,
                    Types = g.Select(t => new SignalTypeDto { Name = t.Name, Count = t.Count }).ToList()
                })
                .OrderBy(c => c.Category)
                .ToList();

            return Result<List<SignalCategoryDto>>.Success(grouped).ToHandlerResult();
        }
    }
}