using ApplicationLayer.Dto.SignalsRetention;
using ApplicationLayer.Features.SignalsRetention.Commands;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.SignalsRetention.Handler
{
    public class PurgeOldSignalsHandler : IRequestHandler<PurgeOldSignalsCommand, PurgeOldSignalsResultDto>
    {
        private readonly IRepository<Signal> _repo;
        private readonly IUnitOfWork _uow;

        public PurgeOldSignalsHandler(IRepository<Signal> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<PurgeOldSignalsResultDto> Handle(PurgeOldSignalsCommand request, CancellationToken cancellationToken)
        {
            var tfAll = new List<string> { "1m", "5m", "1h", "4h", "1d" };
            var tfs = (request.Timeframes != null && request.Timeframes.Count > 0) ? request.Timeframes : tfAll;
            var until = request.Until ?? DateTime.UtcNow;
            var excludeCats = request.ExcludeCategories ?? new List<string>();
            var excludeNames = request.ExcludeSignalNames ?? new List<string>();

            var result = new PurgeOldSignalsResultDto();

            async Task<int> PurgeAsync(string tf)
            {
                var q = _repo.Query().Where(s => s.Timeframe == tf && s.SignalTime < until);
                if (excludeCats.Count > 0) q = q.Where(s => !excludeCats.Contains(s.SignalCategory));
                if (excludeNames.Count > 0) q = q.Where(s => !excludeNames.Contains(s.SignalName));
                var count = await q.CountAsync(cancellationToken);
                if (request.DryRun || count == 0) return count;
                var deleted = await q.ExecuteDeleteAsync(cancellationToken);
                return deleted;
            }

            foreach (var tf in tfs)
            {
                var deleted = await PurgeAsync(tf);
                switch (tf)
                {
                    case "1m": result.Deleted_1m = deleted; break;
                    case "5m": result.Deleted_5m = deleted; break;
                    case "1h": result.Deleted_1h = deleted; break;
                    case "4h": result.Deleted_4h = deleted; break;
                    case "1d": result.Deleted_1d = deleted; break;
                }
            }

            if (!request.DryRun)
                await _uow.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}