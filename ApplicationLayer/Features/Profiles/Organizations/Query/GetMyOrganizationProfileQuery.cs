using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Query;

public record GetMyOrganizationProfileQuery() : IRequest<HandlerResult>;