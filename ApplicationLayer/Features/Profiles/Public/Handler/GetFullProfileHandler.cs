using ApplicationLayer.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Features.Profiles.Public.Query;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Profiles;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Public.Handler;

public class GetFullProfileHandler(IPublicProfileService _publicSvc, IUserContextService _userCtx) : IRequestHandler<GetFullProfileQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetFullProfileQuery request, CancellationToken cancellationToken)
    {
        var uid = request.UserId ?? _userCtx.UserId;
        if (!uid.HasValue || uid.Value <= 0)
            return Result.Failure(Error.Authentication()).ToHandlerResult();

        var result = await _publicSvc.GetByUserIdAsync(uid.Value);
        return result.ToHandlerResult();
    }
}
