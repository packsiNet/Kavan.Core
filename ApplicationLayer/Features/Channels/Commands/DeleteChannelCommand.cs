using MediatR;

namespace ApplicationLayer.Features.Channels.Commands;

public record DeleteChannelCommand(int Id) : IRequest<HandlerResult>;