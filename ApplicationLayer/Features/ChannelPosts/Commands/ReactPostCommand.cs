using ApplicationLayer.DTOs.ChannelPosts;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Commands;

public record ReactPostCommand(ReactPostDto Model) : IRequest<HandlerResult>;