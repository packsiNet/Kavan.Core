using ApplicationLayer.DTOs.ChannelPosts;
using MediatR;

namespace ApplicationLayer.Features.ChannelPosts.Query;

public record GetPostsByChannelQuery(int ChannelId, GetPostsRequestDto Model) : IRequest<HandlerResult>;