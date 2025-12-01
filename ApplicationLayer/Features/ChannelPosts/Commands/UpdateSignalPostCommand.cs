using ApplicationLayer.DTOs.ChannelPosts;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Commands;

public record UpdateSignalPostCommand(int Id, UpdateSignalPostDto Model) : IRequest<HandlerResult>;