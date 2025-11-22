using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Organizations.Commands;
using ApplicationLayer.Interfaces.Profiles;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Handler;

public class UploadLogoHandler(IOrganizationProfileService _service) : IRequestHandler<UploadLogoCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UploadLogoCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UploadLogoAsync(request.File);
        return result.ToHandlerResult();
    }
}