using MediatR;

namespace ApplicationLayer.Features.Follows.Commands;

public record FollowUserCommand(int TargetUserId) : IRequest<HandlerResult>;

