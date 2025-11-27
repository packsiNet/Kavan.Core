using MediatR;

namespace ApplicationLayer.Features.Channels.Commands;

public record JoinChannelCommand(int ChannelId) : IRequest<HandlerResult>;