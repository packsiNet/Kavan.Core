using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.IdeaComments;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.IdeaComments;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.IdeaComments;

[InjectAsScoped]
public class IdeaCommentService(
    IUnitOfWork _uow,
    IRepository<IdeaComment> _comments,
    IRepository<Idea> _ideas,
    IUserContextService _userContext) : IIdeaCommentService
{
    public async Task<Result<IdeaCommentDto>> AddCommentAsync(CreateIdeaCommentDto dto)
    {
        if (_userContext.UserId == null)
            return Result<IdeaCommentDto>.AuthenticationFailure();

        var idea = await _ideas.GetByIdAsync(dto.IdeaId);
        if (idea == null)
            return Result<IdeaCommentDto>.NotFound("ایده یافت نشد");

        if (string.IsNullOrWhiteSpace(dto.Comment))
            return Result<IdeaCommentDto>.ValidationFailure("متن نظر نمی‌تواند خالی باشد");

        var entity = new IdeaComment
        {
            IdeaId = dto.IdeaId,
            UserId = _userContext.UserId.Value,
            Comment = dto.Comment
        };

        await _uow.BeginTransactionAsync();
        await _comments.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        var created = await _comments.GetDbSet()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == entity.Id);
            
        return Result<IdeaCommentDto>.Success(ToDto(created));
    }

    public async Task<Result<List<IdeaCommentDto>>> GetCommentsAsync(int ideaId)
    {
        var comments = await _comments.GetDbSet()
            .Where(x => x.IdeaId == ideaId)
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var dtos = comments.Select(ToDto).ToList();
        return Result<List<IdeaCommentDto>>.Success(dtos);
    }

    public async Task<Result> DeleteCommentAsync(int id)
    {
        var entity = await _comments.GetByIdAsync(id);
        if (entity == null)
            return Result.NotFound("نظر یافت نشد");

        if (_userContext.UserId == null || entity.UserId != _userContext.UserId)
            return Result.Failure(new Error("INCORRECT_USER", "اجازه حذف این نظر را ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));

        await _uow.BeginTransactionAsync();
        _comments.Remove(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result.Success();
    }

    private static IdeaCommentDto ToDto(IdeaComment entity)
    {
        return new IdeaCommentDto
        {
            Id = entity.Id,
            IdeaId = entity.IdeaId,
            UserId = entity.UserId,
            UserName = entity.User?.UserName ?? "Unknown",
            UserAvatar = entity.User?.Avatar,
            Comment = entity.Comment,
            CreatedAt = entity.CreatedAt
        };
    }
}
