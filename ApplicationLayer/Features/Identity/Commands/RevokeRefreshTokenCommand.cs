using MediatR;

namespace ApplicationLayer.Features.Identity.Commands;

public record RevokeRefreshTokenCommand(int UserId) : IRequest<HandlerResult>;