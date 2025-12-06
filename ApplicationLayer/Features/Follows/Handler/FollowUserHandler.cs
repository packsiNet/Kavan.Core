using ApplicationLayer;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Follows.Commands;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Follows.Handler;

public class FollowUserHandler(IRepository<UserFollow> followRepo, IRepository<UserAccount> userRepo, IUnitOfWork uow, IUserContextService userCtx)
    : IRequestHandler<FollowUserCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        var uid = userCtx.UserId;
        if (uid is null)
            return Result.Failure(Error.Authentication()).ToHandlerResult();

        if (request.TargetUserId <= 0)
            return Result.Failure(Error.Validation("شناسه کاربر معتبر نیست")).ToHandlerResult();

        if (uid.Value == request.TargetUserId)
            return Result.Failure(Error.Validation("کاربر نمی‌تواند خودش را فالو کند")).ToHandlerResult();

        var targetExists = await userRepo.Query().AnyAsync(x => x.Id == request.TargetUserId, cancellationToken);
        if (!targetExists)
            return Result.Failure(Error.NotFound("کاربر مقصد یافت نشد")).ToHandlerResult();

        var exists = await followRepo.Query()
            .AnyAsync(x => x.FollowerUserId == uid.Value && x.FolloweeUserId == request.TargetUserId, cancellationToken);
        if (exists)
            return Result.Failure(Error.Duplicate("قبلاً فالو شده است")).ToHandlerResult();

        await uow.BeginTransactionAsync();
        await followRepo.AddAsync(new UserFollow
        {
            FollowerUserId = uid.Value,
            FolloweeUserId = request.TargetUserId
        });
        await uow.SaveChangesAsync();
        await uow.CommitAsync();

        return Result.Success().ToHandlerResult();
    }
}

