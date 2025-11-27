using ApplicationLayer.DTOs.ChannelPosts;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Commands;

public record CreateNewsPostCommand(CreateNewsPostDto Model) : IRequest<HandlerResult>;