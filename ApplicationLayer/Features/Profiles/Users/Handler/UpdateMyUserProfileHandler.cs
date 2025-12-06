using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Profiles.Users.Commands;
using ApplicationLayer.Interfaces;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Users.Handler;

public class UpdateMyUserProfileHandler(IUserAccountServices _svc) : IRequestHandler<UpdateMyUserProfileCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdateMyUserProfileCommand request, CancellationToken cancellationToken)
    {
        var r = await _svc.UpdateUserProfileAsync(request.Model);
        return r.ToResult().ToHandlerResult();
    }
}
