using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Organizations.Query;
using ApplicationLayer.Interfaces.Profiles;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Handler;

public class SearchOrganizationsHandler(IOrganizationProfileService _service) : IRequestHandler<SearchOrganizationsQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(SearchOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var result = await _service.SearchAsync(request.Model);
        return result.ToHandlerResult();
    }
}