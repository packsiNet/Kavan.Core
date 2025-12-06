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

public class GetFollowingHandler(IRepository<UserFollow> followRepo, IRepository<UserAccount> userRepo, IRepository<UserProfile> profileRepo)
    : IRequestHandler<GetFollowingQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
            return Result<List<FollowSummaryDto>>.Failure(Error.Validation("شناسه کاربر معتبر نیست")).ToHandlerResult();

        var query = from f in followRepo.Query()
                    join u in userRepo.Query() on f.FolloweeUserId equals u.Id
                    join p in profileRepo.Query() on u.Id equals p.UserAccountId into up
                    from p in up.DefaultIfEmpty()
                    where f.FollowerUserId == request.UserId
                    orderby f.CreatedAt descending
                    select new FollowSummaryDto
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        DisplayName = p != null ? p.DisplayName : null,
                        Avatar = u.Avatar
                    };

        var list = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
        return Result<List<FollowSummaryDto>>.Success(list).ToHandlerResult();
    }
}

