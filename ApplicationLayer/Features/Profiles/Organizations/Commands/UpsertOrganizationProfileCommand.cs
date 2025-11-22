using ApplicationLayer.DTOs.Profiles.Organizations;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Commands;

public record UpsertOrganizationProfileCommand(UpdateOrganizationProfileDto Model) : IRequest<HandlerResult>;