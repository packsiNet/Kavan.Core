using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Signals;
using MediatR;

namespace ApplicationLayer.Features.Signals.Commands;

public record CreateSignalCommand(string Symbol) : IRequest<HandlerResult>;