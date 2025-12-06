using MediatR;

namespace ApplicationLayer.Features.Profiles.Users.Query;

public record GetMyUserProfileQuery() : IRequest<HandlerResult>;
