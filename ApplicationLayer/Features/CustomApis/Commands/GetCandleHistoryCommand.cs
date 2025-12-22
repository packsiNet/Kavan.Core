using MediatR;

namespace ApplicationLayer.Features.CustomApis.Commands;

public record GetCandleHistoryCommand(DateTime StartDateUtc) : IRequest<HandlerResult>;