using ApplicationLayer.Dto.SignalsRetention;
using MediatR;

namespace ApplicationLayer.Features.SignalsRetention.Commands
{
    public record PurgeOldSignalsCommand(
        List<string> Timeframes,
        DateTime? Until,
        bool DryRun,
        List<string> ExcludeCategories,
        List<string> ExcludeSignalNames
    ) : IRequest<PurgeOldSignalsResultDto>;
}