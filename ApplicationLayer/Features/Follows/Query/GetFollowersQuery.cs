using MediatR;

namespace ApplicationLayer.Features.Follows.Query;

public record GetFollowersQuery(int UserId, int Page = 1, int PageSize = 20) : IRequest<HandlerResult>;

