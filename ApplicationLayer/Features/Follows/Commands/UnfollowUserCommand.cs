using MediatR;

namespace ApplicationLayer.Features.Follows.Commands;

public record UnfollowUserCommand(int TargetUserId) : IRequest<HandlerResult>;

