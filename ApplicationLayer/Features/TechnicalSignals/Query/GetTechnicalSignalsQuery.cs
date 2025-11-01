using ApplicationLayer.Dto.TechnicalSignals;
using MediatR;

namespace ApplicationLayer.Features.TechnicalSignals.Query;

public record GetTechnicalSignalsQuery(TechnicalSignalFilterDto Filter) : IRequest<IEnumerable<TechnicalSignalDto>>;

public record GetTechnicalSignalsSummaryQuery(string? Symbol = null, string? TimeFrame = null) : IRequest<IEnumerable<TechnicalSignalSummaryDto>>;

public record GetTechnicalSignalCategoriesQuery() : IRequest<IEnumerable<string>>;

public record GetTechnicalSignalIndicatorsQuery(string? Category = null) : IRequest<IEnumerable<string>>;