using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Organizations.Commands;
using ApplicationLayer.Interfaces.Profiles;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Handler;

public class UploadBannerHandler(IOrganizationProfileService _service) : IRequestHandler<UploadBannerCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UploadBannerCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UploadBannerAsync(request.File);
        return result.ToHandlerResult();
    }
}