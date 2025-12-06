using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Users.Query;
using ApplicationLayer.Interfaces;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Users.Handler;

public class GetMyUserProfileHandler(IUserAccountServices _svc) : IRequestHandler<GetMyUserProfileQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetMyUserProfileQuery request, CancellationToken cancellationToken)
    {
        var r = await _svc.GetMyUserProfileAsync();
        return r.ToResult<ApplicationLayer.DTOs.User.MyUserProfileDto>().ToHandlerResult();
    }
}
