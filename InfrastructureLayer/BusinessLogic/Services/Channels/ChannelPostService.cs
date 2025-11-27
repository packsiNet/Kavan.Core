using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.ChannelPosts;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Channels;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Channels;

[InjectAsScoped]
public class ChannelPostService(IUnitOfWork _uow,
                                IRepository<Channel> _channels,
                                IRepository<ChannelMembership> _members,
                                IRepository<ChannelPost> _posts,
                                IRepository<ChannelSignalDetail> _signalDetails,
                                IRepository<ChannelSignalEntry> _signalEntries,
                                IRepository<ChannelSignalTp> _signalTps,
                                IRepository<ChannelNewsDetail> _newsDetails,
                                IRepository<ChannelPostReaction> _reactions,
                                ApplicationLayer.Interfaces.External.IFileStorageService _files,
                                IUserContextService _user) : IChannelPostService
{
    public async Task<Result<PostDto>> CreateSignalAsync(CreateSignalPostDto dto)
    {
        if (_user.UserId == null) return Result<PostDto>.AuthenticationFailure();
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == dto.ChannelId);
        if (ch == null) return Result<PostDto>.NotFound("کانال یافت نشد");
        if (ch.OwnerUserId != _user.UserId.Value) return Result<PostDto>.Failure(new Error("INCORRECT_USER", "اجازه ایجاد پست ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        if (!ApplicationLayer.Common.Enums.TradeType.IsValid(dto.TradeType)) return Result<PostDto>.ValidationFailure("نوع معامله نامعتبر است");
        if (dto.EntryPoints == null || dto.EntryPoints.Count == 0) return Result<PostDto>.ValidationFailure("نقطه ورود الزامی است");
        if (dto.Tps == null || dto.Tps.Count == 0) return Result<PostDto>.ValidationFailure("تی‌پی الزامی است");

        string imageUrl = string.Empty;
        if (dto.Image != null)
        {
            var save = await _files.SaveIdeaImageAsync(dto.Image);
            if (save.IsFailure) return Result<PostDto>.Failure(save.Error);
            imageUrl = save.Value;
        }

        var post = new ChannelPost { ChannelId = dto.ChannelId, Type = ApplicationLayer.Common.Enums.ChannelPostType.Signal.Name, Title = dto.Title ?? string.Empty, Description = dto.Description ?? string.Empty, ImageUrl = imageUrl };

        await _uow.BeginTransactionAsync();
        await _posts.AddAsync(post);
        await _uow.SaveChangesAsync();

        var det = new ChannelSignalDetail { PostId = post.Id, Symbol = dto.Symbol, Timeframe = dto.Timeframe, TradeType = ApplicationLayer.Common.Enums.TradeType.FromValue(dto.TradeType).Name, StopLoss = dto.StopLoss };
        await _signalDetails.AddAsync(det);
        foreach (var ep in dto.EntryPoints)
            await _signalEntries.AddAsync(new ChannelSignalEntry { PostId = post.Id, Price = ep });
        foreach (var tp in dto.Tps)
            await _signalTps.AddAsync(new ChannelSignalTp { PostId = post.Id, Price = tp });
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result<PostDto>.Success(await ToDtoAsync(post.Id));
    }

    public async Task<Result<PostDto>> CreateNewsAsync(CreateNewsPostDto dto)
    {
        if (_user.UserId == null) return Result<PostDto>.AuthenticationFailure();
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == dto.ChannelId);
        if (ch == null) return Result<PostDto>.NotFound("کانال یافت نشد");
        if (ch.OwnerUserId != _user.UserId.Value) return Result<PostDto>.Failure(new Error("INCORRECT_USER", "اجازه ایجاد پست ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));

        string imageUrl = string.Empty;
        if (dto.Image != null)
        {
            var save = await _files.SaveIdeaImageAsync(dto.Image);
            if (save.IsFailure) return Result<PostDto>.Failure(save.Error);
            imageUrl = save.Value;
        }

        var post = new ChannelPost { ChannelId = dto.ChannelId, Type = ApplicationLayer.Common.Enums.ChannelPostType.News.Name, Title = dto.Title ?? string.Empty, Description = dto.Description ?? string.Empty, ImageUrl = imageUrl };

        await _uow.BeginTransactionAsync();
        await _posts.AddAsync(post);
        await _uow.SaveChangesAsync();
        await _newsDetails.AddAsync(new ChannelNewsDetail { PostId = post.Id, Url = dto.Url ?? string.Empty });
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result<PostDto>.Success(await ToDtoAsync(post.Id));
    }

    public async Task<Result<PostDto>> UpdateSignalAsync(int postId, UpdateSignalPostDto dto)
    {
        if (_user.UserId == null) return Result<PostDto>.AuthenticationFailure();
        var post = await _posts.GetDbSet().FirstOrDefaultAsync(x => x.Id == postId);
        if (post == null) return Result<PostDto>.NotFound("پست یافت نشد");
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == post.ChannelId);
        if (ch.OwnerUserId != _user.UserId.Value) return Result<PostDto>.Failure(new Error("INCORRECT_USER", "اجازه ویرایش پست ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));

        post.Title = dto.Title ?? string.Empty;
        post.Description = dto.Description ?? string.Empty;
        post.IsActive = dto.IsActive;
        var det = await _signalDetails.GetDbSet().FirstOrDefaultAsync(x => x.PostId == postId);
        det.Symbol = dto.Symbol;
        det.Timeframe = dto.Timeframe;
        det.TradeType = ApplicationLayer.Common.Enums.TradeType.FromValue(dto.TradeType).Name;
        det.StopLoss = dto.StopLoss;

        var existingEntries = await _signalEntries.Query().Where(x => x.PostId == postId).ToListAsync();
        var existingTps = await _signalTps.Query().Where(x => x.PostId == postId).ToListAsync();

        await _uow.BeginTransactionAsync();
        foreach (var e in existingEntries) _signalEntries.Remove(e);
        foreach (var t in existingTps) _signalTps.Remove(t);
        await _posts.UpdateAsync(post);
        await _signalDetails.UpdateAsync(det);
        foreach (var ep in dto.EntryPoints) await _signalEntries.AddAsync(new ChannelSignalEntry { PostId = postId, Price = ep });
        foreach (var tp in dto.Tps) await _signalTps.AddAsync(new ChannelSignalTp { PostId = postId, Price = tp });
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result<PostDto>.Success(await ToDtoAsync(postId));
    }

    public async Task<Result<PostDto>> UpdateNewsAsync(int postId, UpdateNewsPostDto dto)
    {
        if (_user.UserId == null) return Result<PostDto>.AuthenticationFailure();
        var post = await _posts.GetDbSet().FirstOrDefaultAsync(x => x.Id == postId);
        if (post == null) return Result<PostDto>.NotFound("پست یافت نشد");
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == post.ChannelId);
        if (ch.OwnerUserId != _user.UserId.Value) return Result<PostDto>.Failure(new Error("INCORRECT_USER", "اجازه ویرایش پست ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));

        post.Title = dto.Title ?? string.Empty;
        post.Description = dto.Description ?? string.Empty;
        post.IsActive = dto.IsActive;
        var det = await _newsDetails.GetDbSet().FirstOrDefaultAsync(x => x.PostId == postId);
        det.Url = dto.Url ?? string.Empty;

        await _uow.BeginTransactionAsync();
        await _posts.UpdateAsync(post);
        await _newsDetails.UpdateAsync(det);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result<PostDto>.Success(await ToDtoAsync(postId));
    }

    public async Task<Result> DeleteAsync(int postId)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        var post = await _posts.GetDbSet().FirstOrDefaultAsync(x => x.Id == postId);
        if (post == null) return Result.NotFound("پست یافت نشد");
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == post.ChannelId);
        if (ch.OwnerUserId != _user.UserId.Value) return Result.Failure(new Error("INCORRECT_USER", "اجازه حذف پست ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));

        await _uow.BeginTransactionAsync();
        _posts.Remove(post);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    public async Task<Result<PostsPageDto>> GetByChannelAsync(int channelId, GetPostsRequestDto dto)
    {
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == channelId);
        if (ch == null) return Result<PostsPageDto>.NotFound("کانال یافت نشد");
        var isPaid = ch.AccessType == ApplicationLayer.Common.Enums.ChannelAccessType.Paid.Name;
        if (isPaid)
        {
            if (_user.UserId == null) return Result<PostsPageDto>.AuthenticationFailure();
            var member = await _members.GetDbSet().FirstOrDefaultAsync(x => x.ChannelId == channelId && x.UserId == _user.UserId.Value && x.IsActive);
            if (member == null) return Result<PostsPageDto>.Failure(new Error("FORBIDDEN", "دسترسی به پست‌ها برای اعضا است", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        }

        var query = _posts.Query().Where(x => x.ChannelId == channelId && x.IsActive);
        var total = await query.CountAsync();
        var posts = await query.OrderByDescending(x => x.CreatedAt).Skip(dto.Pagination.Skip).Take(dto.Pagination.PageSize).Select(x => x.Id).ToListAsync();
        var items = new List<PostDto>();
        foreach (var id in posts)
            items.Add(await ToDtoAsync(id));
        var page = new PostsPageDto { Items = items, Total = total, Page = dto.Pagination.Page, PageSize = dto.Pagination.PageSize };
        return Result<PostsPageDto>.Success(page);
    }

    public async Task<Result<PostDto>> ReactAsync(ReactPostDto dto)
    {
        if (_user.UserId == null) return Result<PostDto>.AuthenticationFailure();
        var post = await _posts.GetDbSet().FirstOrDefaultAsync(x => x.Id == dto.PostId);
        if (post == null) return Result<PostDto>.NotFound("پست یافت نشد");
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == post.ChannelId);
        var isPaid = ch.AccessType == ApplicationLayer.Common.Enums.ChannelAccessType.Paid.Name;
        var member = await _members.GetDbSet().FirstOrDefaultAsync(x => x.ChannelId == ch.Id && x.UserId == _user.UserId.Value && (!isPaid || x.IsActive));
        if (member == null) return Result<PostDto>.Failure(new Error("FORBIDDEN", "فقط اعضا می‌توانند واکنش ثبت کنند", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        var rx = await _reactions.GetDbSet().FirstOrDefaultAsync(x => x.PostId == dto.PostId && x.UserId == _user.UserId.Value);
        if (rx == null)
        {
            rx = new ChannelPostReaction { PostId = dto.PostId, UserId = _user.UserId.Value, Reaction = dto.Reaction };
            await _uow.BeginTransactionAsync();
            await _reactions.AddAsync(rx);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();
        }
        else
        {
            rx.Reaction = dto.Reaction;
            rx.MarkAsUpdated();
            await _uow.BeginTransactionAsync();
            await _reactions.UpdateAsync(rx);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();
        }
        return Result<PostDto>.Success(await ToDtoAsync(dto.PostId));
    }

    private async Task<PostDto> ToDtoAsync(int postId)
    {
        var post = await _posts.GetDbSet().FirstOrDefaultAsync(x => x.Id == postId);
        var likes = await _reactions.Query().Where(x => x.PostId == postId && x.Reaction == "like").CountAsync();
        var dislikes = await _reactions.Query().Where(x => x.PostId == postId && x.Reaction == "dislike").CountAsync();
        var dto = new PostDto
        {
            Id = post.Id,
            ChannelId = post.ChannelId,
            Type = post.Type,
            Title = post.Title,
            Description = post.Description,
            Image = post.ImageUrl,
            CreatedAt = post.CreatedAt,
            Likes = likes,
            Dislikes = dislikes
        };
        if (post.Type == ApplicationLayer.Common.Enums.ChannelPostType.Signal.Name)
        {
            var det = await _signalDetails.GetDbSet().FirstOrDefaultAsync(x => x.PostId == postId);
            var entries = await _signalEntries.Query().Where(x => x.PostId == postId).Select(x => x.Price).ToListAsync();
            var tps = await _signalTps.Query().Where(x => x.PostId == postId).Select(x => x.Price).ToListAsync();
            dto.SignalDetail = new SignalDetailDto
            {
                Symbol = det.Symbol,
                Timeframe = det.Timeframe,
                TradeType = det.TradeType,
                EntryPoints = entries,
                Tps = tps,
                StopLoss = det.StopLoss
            };
        }
        else
        {
            var det = await _newsDetails.GetDbSet().FirstOrDefaultAsync(x => x.PostId == postId);
            dto.NewsDetail = new NewsDetailDto { Url = det.Url };
        }
        return dto;
    }
}