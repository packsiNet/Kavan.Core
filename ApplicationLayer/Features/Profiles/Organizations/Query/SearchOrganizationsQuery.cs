using ApplicationLayer.DTOs.Profiles.Organizations;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Query;

public record SearchOrganizationsQuery(SearchOrganizationsDto Model) : IRequest<HandlerResult>;