using ApplicationLayer;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Follows;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Follows.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Follows.Handler;

public class IsFollowingHandler(IRepository<UserFollow> followRepo, IUserContextService userCtx)
    : IRequestHandler<IsFollowingQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(IsFollowingQuery request, CancellationToken cancellationToken)
    {
        var uid = userCtx.UserId;
        if (uid is null)
            return Result<FollowStatusDto>.Failure(Error.Authentication()).ToHandlerResult();

        if (request.TargetUserId <= 0)
            return Result<FollowStatusDto>.Failure(Error.Validation("شناسه کاربر معتبر نیست")).ToHandlerResult();

        var isFollowing = await followRepo.Query()
            .AnyAsync(x => x.FollowerUserId == uid.Value && x.FolloweeUserId == request.TargetUserId, cancellationToken);

        var followersCount = await followRepo.Query()
            .CountAsync(x => x.FolloweeUserId == request.TargetUserId, cancellationToken);

        var followingCount = await followRepo.Query()
            .CountAsync(x => x.FollowerUserId == request.TargetUserId, cancellationToken);

        var dto = new FollowStatusDto
        {
            IsFollowing = isFollowing,
            FollowersCount = followersCount,
            FollowingCount = followingCount
        };

        return Result<FollowStatusDto>.Success(dto).ToHandlerResult();
    }
}

