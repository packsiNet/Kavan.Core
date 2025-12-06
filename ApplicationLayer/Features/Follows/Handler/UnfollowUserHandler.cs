using ApplicationLayer;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Follows.Commands;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Follows.Handler;

public class UnfollowUserHandler(IRepository<UserFollow> followRepo, IUnitOfWork uow, IUserContextService userCtx)
    : IRequestHandler<UnfollowUserCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var uid = userCtx.UserId;
        if (uid is null)
            return Result.Failure(Error.Authentication()).ToHandlerResult();

        if (request.TargetUserId <= 0)
            return Result.Failure(Error.Validation("شناسه کاربر معتبر نیست")).ToHandlerResult();

        var entity = await followRepo.Query()
            .FirstOrDefaultAsync(x => x.FollowerUserId == uid.Value && x.FolloweeUserId == request.TargetUserId, cancellationToken);

        if (entity is null)
            return Result.Failure(Error.NotFound("رابطه فالو یافت نشد")).ToHandlerResult();

        await uow.BeginTransactionAsync();
        followRepo.Remove(entity);
        uow.SaveChanges();
        await uow.CommitAsync();

        return Result.Success().ToHandlerResult();
    }
}

