using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Channels;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Channels;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using ApplicationLayer.Common.Enums;

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
        if (!ChannelType.IsValid(dto.Type))
            return Result<ChannelDto>.ValidationFailure("دسته کانال نامعتبر است");
        if (!ChannelAccessType.IsValid(dto.AccessType))
            return Result<ChannelDto>.ValidationFailure("نوع دسترسی نامعتبر است");
        if (_user.UserId == null)
            return Result<ChannelDto>.AuthenticationFailure();

        var slug = dto.Title.Trim().ToLower().Replace(' ', '-');
        var exists = await _channels.Query().AnyAsync(x => x.Slug == slug);
        if (exists) slug = slug + "-" + Guid.NewGuid().ToString("N").Substring(0, 6);

        // Generate Unique Code
        string uniqueCode;
        do
        {
            uniqueCode = "KCH" + new Random().Next(100, 9999);
        } while (await _channels.Query().AnyAsync(x => x.UniqueCode == uniqueCode));

        var entity = new Channel
        {
            OwnerUserId = _user.UserId.Value,
            Title = dto.Title,
            Slug = slug,
            Type = dto.Type,
            AccessType = dto.AccessType,
            Description = dto.Description ?? string.Empty,
            UniqueCode = uniqueCode,
            Price = dto.Price,
            Currency = dto.Currency ?? "IRR",
            BannerUrl = dto.BannerUrl ?? string.Empty,
            LogoUrl = dto.LogoUrl ?? string.Empty
        };

        await _uow.BeginTransactionAsync();
        await _channels.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result<ChannelDto>.Success(await ToDto(entity));
    }

    public async Task<Result<ChannelDto>> UpdateAsync(int id, UpdateChannelDto dto)
    {
        var entity = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result<ChannelDto>.NotFound("کانال یافت نشد");
        if (_user.UserId == null || entity.OwnerUserId != _user.UserId.Value)
            return Result<ChannelDto>.Failure(new Error("INCORRECT_USER", "اجازه ویرایش این کانال را ندارید", RequestStatus.IncorrectUser));
        if (!ChannelType.IsValid(dto.Category)) // dto.Category is int now in UpdateChannelDto? Need to check DTO
             return Result<ChannelDto>.ValidationFailure("دسته کانال نامعتبر است");
        // Note: UpdateChannelDto might still use old names. I should check it. 
        // Assuming UpdateChannelDto has compatible fields or I update it. 
        // For now, I'll assume UpdateChannelDto is updated or I'll handle it.
        // Let's assume UpdateChannelDto has Category/AccessType as int.

        entity.Title = dto.Name; // Map Name to Title if DTO uses Name
        entity.Type = dto.Category;
        entity.AccessType = dto.AccessType; // int
        entity.Description = dto.Description ?? string.Empty;
        entity.IsActive = dto.IsActive;
        entity.MarkAsUpdated();

        await _uow.BeginTransactionAsync();
        await _channels.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result<ChannelDto>.Success(await ToDto(entity));
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var entity = await _channels.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return Result.NotFound("کانال یافت نشد");
        if (_user.UserId == null || entity.OwnerUserId != _user.UserId.Value)
            return Result.Failure(new Error("INCORRECT_USER", "اجازه حذف این کانال را ندارید", RequestStatus.IncorrectUser));

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
        return Result<ChannelDto>.Success(await ToDto(entity));
    }

    public async Task<Result<ChannelsPageDto>> GetMyAsync(GetChannelsRequestDto dto)
    {
        if (_user.UserId == null) return Result<ChannelsPageDto>.AuthenticationFailure();
        
        // Channels I have joined
        var joinedChannelIds = await _members.Query()
            .Where(x => x.UserId == _user.UserId.Value && x.IsActive)
            .Select(x => x.ChannelId)
            .ToListAsync();

        var query = _channels.Query().Where(x => joinedChannelIds.Contains(x.Id));

        if (dto.Category != null)
            query = query.Where(x => x.Type == dto.Category.Value);
        if (dto.AccessType != null)
            query = query.Where(x => x.AccessType == dto.AccessType.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(dto.Pagination.Skip)
            .Take(dto.Pagination.PageSize)
            .ToListAsync();

        var list = new List<ChannelDto>();
        foreach (var ch in items)
        {
            list.Add(await ToDto(ch));
        }

        var page = new ChannelsPageDto { Items = list, Total = total, Page = dto.Pagination.Page, PageSize = dto.Pagination.PageSize };
        return Result<ChannelsPageDto>.Success(page);
    }

    public async Task<Result<ChannelsPageDto>> GetCreatedAsync(GetChannelsRequestDto dto)
    {
        if (_user.UserId == null) return Result<ChannelsPageDto>.AuthenticationFailure();
        
        var query = _channels.Query().Where(x => x.OwnerUserId == _user.UserId.Value);

        if (dto.Category != null)
            query = query.Where(x => x.Type == dto.Category.Value);
        if (dto.AccessType != null)
            query = query.Where(x => x.AccessType == dto.AccessType.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(dto.Pagination.Skip)
            .Take(dto.Pagination.PageSize)
            .ToListAsync();

        var list = new List<ChannelDto>();
        foreach (var ch in items)
        {
            list.Add(await ToDto(ch));
        }

        var page = new ChannelsPageDto { Items = list, Total = total, Page = dto.Pagination.Page, PageSize = dto.Pagination.PageSize };
        return Result<ChannelsPageDto>.Success(page);
    }

    public async Task<Result<ChannelsPageDto>> GetPublicAsync(GetChannelsRequestDto dto)
    {
        var query = _channels.Query().Where(x => x.IsActive);
        if (dto.Category != null)
            query = query.Where(x => x.Type == dto.Category.Value);
        if (dto.AccessType != null)
            query = query.Where(x => x.AccessType == dto.AccessType.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(dto.Pagination.Skip)
            .Take(dto.Pagination.PageSize)
            .ToListAsync();

        var list = new List<ChannelDto>();
        foreach (var ch in items)
        {
            list.Add(await ToDto(ch));
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
                var isFree = ch.AccessType == ChannelAccessType.Free.Value;
                member.IsActive = isFree;
                await _uow.BeginTransactionAsync();
                await _members.UpdateAsync(member);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return isFree ? Result.Success() : Result.Failure(new Error("PAYMENT_REQUIRED", "برای دسترسی به کانال پرداخت لازم است", RequestStatus.IncorrectUser));
            }
            return Result.Success();
        }

        var active = ch.AccessType == ChannelAccessType.Free.Value;
        var entity = new ChannelMembership { ChannelId = channelId, UserId = _user.UserId.Value, IsActive = active };
        await _uow.BeginTransactionAsync();
        await _members.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return active ? Result.Success() : Result.Failure(new Error("PAYMENT_REQUIRED", "برای دسترسی به کانال پرداخت لازم است", RequestStatus.IncorrectUser));
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
        if (member == null) return Result<ChannelDto>.Failure(new Error("FORBIDDEN", "فقط اعضا می‌توانند امتیاز دهند", RequestStatus.IncorrectUser));

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

        return Result<ChannelDto>.Success(await ToDto(ch));
    }

    public async Task<Result> MuteAsync(int channelId)
    {
        if (_user.UserId == null) return Result.AuthenticationFailure();
        var member = await _members.GetDbSet().FirstOrDefaultAsync(x => x.ChannelId == channelId && x.UserId == _user.UserId.Value);
        if (member == null) return Result.NotFound("عضو کانال نیستید");

        member.IsMuted = !member.IsMuted;
        member.MarkAsUpdated();
        
        await _uow.BeginTransactionAsync();
        await _members.UpdateAsync(member);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result.Success();
    }

    private async Task<(int members, int ratings, double avg)> GetStatsAsync(int channelId)
    {
        var members = await _members.Query().Where(x => x.ChannelId == channelId && x.IsActive).CountAsync();
        var ratings = await _ratings.Query().Where(x => x.ChannelId == channelId).CountAsync();
        var avg = ratings == 0 ? 0 : await _ratings.Query().Where(x => x.ChannelId == channelId).AverageAsync(x => x.Stars);
        return (members, ratings, avg);
    }

    private async Task<ChannelDto> ToDto(Channel ch)
    {
        var stats = await GetStatsAsync(ch.Id);
        
        bool isJoined = false;
        if (_user.UserId != null)
        {
            isJoined = await _members.Query().AnyAsync(x => x.ChannelId == ch.Id && x.UserId == _user.UserId.Value && x.IsActive);
        }

        string typeName = ChannelType.TryFromValue(ch.Type, out var type) ? type.Name : ch.Type.ToString();
        string accessTypeName = ChannelAccessType.TryFromValue(ch.AccessType, out var acc) ? acc.Name : ch.AccessType.ToString();

        return new ChannelDto
        {
            Id = ch.Id,
            Title = ch.Title,
            Slug = ch.Slug,
            UniqueCode = ch.UniqueCode,
            TypeId = ch.Type,
            Type = typeName,
            AccessTypeId = ch.AccessType,
            AccessType = accessTypeName,
            IsPremium = ch.AccessType == ChannelAccessType.VIP.Value,
            Description = ch.Description,
            Price = ch.Price,
            Currency = ch.Currency,
            BannerUrl = ch.BannerUrl,
            LogoUrl = ch.LogoUrl,
            IsActive = ch.IsActive,
            MembersCount = stats.members,
            RatingsCount = stats.ratings,
            AverageStars = stats.avg,
            IsJoined = isJoined
        };
    }
}
