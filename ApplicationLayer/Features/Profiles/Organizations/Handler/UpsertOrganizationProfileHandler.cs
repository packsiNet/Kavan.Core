using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Organizations.Commands;
using ApplicationLayer.Interfaces.Profiles;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Handler;

public class UpsertOrganizationProfileHandler(IOrganizationProfileService _service) : IRequestHandler<UpsertOrganizationProfileCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpsertOrganizationProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpsertAsync(request.Model);
        return result.ToHandlerResult();
    }
}