using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Query;

public record GetPublicProfileByUserIdQuery(int UserId) : IRequest<HandlerResult>;