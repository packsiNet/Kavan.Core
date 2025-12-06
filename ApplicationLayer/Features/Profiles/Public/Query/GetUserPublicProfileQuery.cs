using MediatR;

namespace ApplicationLayer.Features.Profiles.Public.Query;

public record GetUserPublicProfileQuery(int UserId) : IRequest<HandlerResult>;
