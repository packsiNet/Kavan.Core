using MediatR;

namespace ApplicationLayer.Features.Follows.Query;

public record IsFollowingQuery(int TargetUserId) : IRequest<HandlerResult>;

