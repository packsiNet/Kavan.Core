using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Commands;

public record DeletePostCommand(int Id) : IRequest<HandlerResult>;