using ApplicationLayer.Dto.BaseDtos;
using MediatR;

namespace ApplicationLayer.Features.Signals.Query;

public record GetSignalsQuery(string? Symbol, string? TimeFrame, int? Limit) : IRequest<HandlerResult>;