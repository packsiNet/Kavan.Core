using MediatR;

namespace ApplicationLayer.Features.CustomApis.Commands;

public record GetCandleHistoryCommand(DateTime StartDateUtc, DateTime? EndDateUtc) : IRequest<HandlerResult>;