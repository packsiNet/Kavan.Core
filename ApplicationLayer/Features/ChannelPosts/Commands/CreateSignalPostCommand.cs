using ApplicationLayer.DTOs.ChannelPosts;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Commands;

public record CreateSignalPostCommand(CreateSignalPostDto Model) : IRequest<HandlerResult>;