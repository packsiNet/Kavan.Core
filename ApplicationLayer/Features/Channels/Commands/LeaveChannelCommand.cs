using MediatR;

namespace ApplicationLayer.Features.Channels.Commands;

public record LeaveChannelCommand(int ChannelId) : IRequest<HandlerResult>;