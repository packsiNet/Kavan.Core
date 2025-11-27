using ApplicationLayer.DTOs.ChannelPosts;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Commands;

public record UpdateNewsPostCommand(int Id, UpdateNewsPostDto Model) : IRequest<HandlerResult>;