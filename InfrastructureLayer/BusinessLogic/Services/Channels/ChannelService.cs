using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Channels;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Channels;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Channels;

[InjectAsScoped]
public class ChannelService(IUnitOfWork _uow,
                            IRepository<Channel> _channels,
                            IRepository<ChannelMembership> _members,
                            IRepository<ChannelRating> _ratings,
                            IUserContextService _user) : IChannelService
{
    public async Task<Result<ChannelDto>> CreateAsync(CreateChannelDto dto)
    {
        if (!ApplicationLayer.Common.Enums.ChannelCategory.IsValid(dto.Category))
            return Result<ChannelDto>.ValidationFailure("دسته کانال نامعتبر است");
        if (!ApplicationLayer.Common.Enums.ChannelAccessType.IsValid(dto.AccessType))
            return Result<ChannelDto>.ValidationFailure("نوع دسترسی نامعتبر است");
        if (_user.UserId == null)
            return Result<ChannelDto>.AuthenticationFailure();

        var cat = ApplicationLayer.Common.Enums.ChannelCategory.FromValue(dto.Category);
        var acc = ApplicationLayer.Common.Enums.ChannelAccessType.FromValue(dto.AccessType);

        var slug = dto.Name.Trim().ToLower().Replace(' ', '-');
        var exists = await _channels.Query().AnyAsync(x => x.Slug == slug);
        if (exists) slug = slug + "-" + Guid.NewGuid().ToString("N").Substring(0, 6);

        var entity = new Channel
        {
            OwnerUserId = _user.UserId.Value,
            Name = dto.Name,
            Slug = slug,
            Category = cat.Name,
            AccessType = acc.Name,
            Description = dto.Description ?? string.Empty
        };

        await _uow.BeginTransactionAsync();
        await _channels.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result<ChannelDto>.Success(ToDto(entity, 0, 0, 0));
    }

    public async Task<Result<ChannelDto>> UpdateAsync(int id, UpdateChannelDto dto)
    {
        var entity = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result<ChannelDto>.NotFound("کانال یافت نشد");
        if (_user.UserId == null || entity.OwnerUserId != _user.UserId.Value)
            return Result<ChannelDto>.Failure(new Error("INCORRECT_USER", "اجازه ویرایش این کانال را ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
        if (!ApplicationLayer.Common.Enums.ChannelCategory.IsValid(dto.Category))
            return Result<ChannelDto>.ValidationFailure("دسته کانال نامعتبر است");
        if (!ApplicationLayer.Common.Enums.ChannelAccessType.IsValid(dto.AccessType))
            return Result<ChannelDto>.ValidationFailure("نوع دسترسی نامعتبر است");

        entity.Name = dto.Name;
        entity.Category = ApplicationLayer.Common.Enums.ChannelCategory.FromValue(dto.Category).Name;
        entity.AccessType = ApplicationLayer.Common.Enums.ChannelAccessType.FromValue(dto.AccessType).Name;
        entity.Description = dto.Description ?? string.Empty;
        entity.IsActive = dto.IsActive;
        entity.MarkAsUpdated();

        await _uow.BeginTransactionAsync();
        await _channels.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        var stats = await GetStatsAsync(entity.Id);
        return Result<ChannelDto>.Success(ToDto(entity, stats.members, stats.ratings, stats.avg));
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var entity = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result.NotFound("کانال یافت نشد");
        if (_user.UserId == null || entity.OwnerUserId != _user.UserId.Value)
            return Result.Failure(new Error("INCORRECT_USER", "اجازه حذف این کانال را ندارید", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));

        await _uow.BeginTransactionAsync();
        _channels.Remove(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    public async Task<Result<ChannelDto>> GetByIdAsync(int id)
    {
        var entity = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result<ChannelDto>.NotFound("کانال یافت نشد");
        var stats = await GetStatsAsync(id);
        return Result<ChannelDto>.Success(ToDto(entity, stats.members, stats.ratings, stats.avg));
    }

    public async Task<Result<ChannelsPageDto>> GetMyAsync(GetChannelsRequestDto dto)
    {
        if (_user.UserId == null) return Result<ChannelsPageDto>.AuthenticationFailure();
        var query = _channels.Query().Where(x => x.OwnerUserId == _user.UserId.Value);
        if (dto.Category != null)
            query = query.Where(x => x.Category == ApplicationLayer.Common.Enums.ChannelCategory.FromValue(dto.Category.Value).Name);
        if (dto.AccessType != null)
            query = query.Where(x => x.AccessType == ApplicationLayer.Common.Enums.ChannelAccessType.FromValue(dto.AccessType.Value).Name);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(dto.Pagination.Skip)
            .Take(dto.Pagination.PageSize)
            .Select(x => x)
            .ToListAsync();

        var list = new List<ChannelDto>();
        foreach (var ch in items)
        {
            var stats = await GetStatsAsync(ch.Id);
            list.Add(ToDto(ch, stats.members, stats.ratings, stats.avg));
        }

        var page = new ChannelsPageDto { Items = list, Total = total, Page = dto.Pagination.Page, PageSize = dto.Pagination.PageSize };
        return Result<ChannelsPageDto>.Success(page);
    }

    public async Task<Result<ChannelsPageDto>> GetPublicAsync(GetChannelsRequestDto dto)
    {
        var query = _channels.Query().Where(x => x.IsActive);
        if (dto.Category != null)
            query = query.Where(x => x.Category == ApplicationLayer.Common.Enums.ChannelCategory.FromValue(dto.Category.Value).Name);
        if (dto.AccessType != null)
            query = query.Where(x => x.AccessType == ApplicationLayer.Common.Enums.ChannelAccessType.FromValue(dto.AccessType.Value).Name);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(dto.Pagination.Skip)
            .Take(dto.Pagination.PageSize)
            .Select(x => x)
            .ToListAsync();

        var list = new List<ChannelDto>();
        foreach (var ch in items)
        {
            var stats = await GetStatsAsync(ch.Id);
            list.Add(ToDto(ch, stats.members, stats.ratings, stats.avg));
        }

        var page = new ChannelsPageDto { Items = list, Total = total, Page = dto.Pagination.Page, PageSize = dto.Pagination.PageSize };
        return Result<ChannelsPageDto>.Success(page);
    }

    public async Task<Result> JoinAsync(int channelId)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == channelId);
        if (ch == null) return Result.NotFound("کانال یافت نشد");

        var member = await _members.GetDbSet().FirstOrDefaultAsync(x => x.ChannelId == channelId && x.UserId == _user.UserId.Value);
        if (member != null)
        {
            if (!member.IsActive)
            {
                var isFree = ch.AccessType == ApplicationLayer.Common.Enums.ChannelAccessType.Free.Name;
                member.IsActive = isFree;
                await _uow.BeginTransactionAsync();
                await _members.UpdateAsync(member);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return isFree ? Result.Success() : Result.Failure(new Error("PAYMENT_REQUIRED", "برای دسترسی به کانال پرداخت لازم است", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
            }
            return Result.Success();
        }

        var active = ch.AccessType == ApplicationLayer.Common.Enums.ChannelAccessType.Free.Name;
        var entity = new ChannelMembership { ChannelId = channelId, UserId = _user.UserId.Value, IsActive = active };
        await _uow.BeginTransactionAsync();
        await _members.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return active ? Result.Success() : Result.Failure(new Error("PAYMENT_REQUIRED", "برای دسترسی به کانال پرداخت لازم است", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));
    }

    public async Task<Result> LeaveAsync(int channelId)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        var member = await _members.GetDbSet().FirstOrDefaultAsync(x => x.ChannelId == channelId && x.UserId == _user.UserId.Value);
        if (member == null) return Result.Success();
        await _uow.BeginTransactionAsync();
        _members.Remove(member);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    public async Task<Result<ChannelDto>> RateAsync(RateChannelDto dto)
    {
        if (_user.UserId == null) return Result<ChannelDto>.AuthenticationFailure();
        if (dto.Stars < 1 || dto.Stars > 5) return Result<ChannelDto>.ValidationFailure("امتیاز نامعتبر است");
        var ch = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == dto.ChannelId);
        if (ch == null) return Result<ChannelDto>.NotFound("کانال یافت نشد");
        var member = await _members.GetDbSet().FirstOrDefaultAsync(x => x.ChannelId == dto.ChannelId && x.UserId == _user.UserId.Value && x.IsActive);
        if (member == null) return Result<ChannelDto>.Failure(new Error("FORBIDDEN", "فقط اعضا می‌توانند امتیاز دهند", ApplicationLayer.Common.Enums.RequestStatus.IncorrectUser));

        var rating = await _ratings.GetDbSet().FirstOrDefaultAsync(x => x.ChannelId == dto.ChannelId && x.UserId == _user.UserId.Value);
        if (rating == null)
        {
            rating = new ChannelRating { ChannelId = dto.ChannelId, UserId = _user.UserId.Value, Stars = dto.Stars };
            await _uow.BeginTransactionAsync();
            await _ratings.AddAsync(rating);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();
        }
        else
        {
            rating.Stars = dto.Stars;
            rating.MarkAsUpdated();
            await _uow.BeginTransactionAsync();
            await _ratings.UpdateAsync(rating);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();
        }

        var stats = await GetStatsAsync(dto.ChannelId);
        return Result<ChannelDto>.Success(ToDto(ch, stats.members, stats.ratings, stats.avg));
    }

    private async Task<(int members, int ratings, double avg)> GetStatsAsync(int channelId)
    {
        var members = await _members.Query().Where(x => x.ChannelId == channelId && x.IsActive).CountAsync();
        var ratings = await _ratings.Query().Where(x => x.ChannelId == channelId).CountAsync();
        var avg = ratings == 0 ? 0 : await _ratings.Query().Where(x => x.ChannelId == channelId).AverageAsync(x => x.Stars);
        return (members, ratings, avg);
    }

    private static ChannelDto ToDto(Channel ch, int members, int ratings, double avg)
    {
        return new ChannelDto
        {
            Id = ch.Id,
            Name = ch.Name,
            Slug = ch.Slug,
            Category = ch.Category,
            AccessType = ch.AccessType,
            Description = ch.Description,
            IsActive = ch.IsActive,
            MembersCount = members,
            RatingsCount = ratings,
            AverageStars = avg
        };
    }
}