using MediatR;

namespace ApplicationLayer.Features.Profiles.Public.Query;

public record GetFullProfileQuery(int? UserId) : IRequest<HandlerResult>;
