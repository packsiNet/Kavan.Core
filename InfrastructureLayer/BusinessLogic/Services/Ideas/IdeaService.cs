using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Ideas;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Ideas;
using ApplicationLayer.Interfaces.External;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Ideas;

[InjectAsScoped]
public class IdeaService(IUnitOfWork _uow,
                         IRepository<Idea> _ideas,
                         IRepository<Cryptocurrency> _cryptos,
                         IFileStorageService _fileStorage) : IIdeaService
{
    public async Task<Result<IdeaDto>> CreateAsync(CreateIdeaDto dto)
    {
        if (!ApplicationLayer.Common.Enums.TimeframeUnit.IsValid(dto.Timeframe))
            return Result<IdeaDto>.ValidationFailure("محدوده زمانی نامعتبر است");
        if (!ApplicationLayer.Common.Enums.IdeaTrend.IsValid(dto.Trend))
            return Result<IdeaDto>.ValidationFailure("روند نامعتبر است");
        if (!ApplicationLayer.Common.Enums.IdeaVisibility.IsValid(dto.Status))
            return Result<IdeaDto>.ValidationFailure("وضعیت نامعتبر است");

        var crypto = await _cryptos.GetDbSet().FirstOrDefaultAsync(x => x.Symbol == dto.Symbol);
        if (crypto == null)
            return Result<IdeaDto>.NotFound("نماد یافت نشد");

        string imageUrl = null;
        if (dto.Image != null)
        {
            var saveImage = await _fileStorage.SaveIdeaImageAsync(dto.Image);
            if (saveImage.IsFailure)
                return Result<IdeaDto>.Failure(saveImage.Error);
            imageUrl = saveImage.Value;
        }

        var entity = new Idea
        {
            Symbol = dto.Symbol,
            CryptocurrencyId = crypto.Id,
            Timeframe = dto.Timeframe,
            Trend = dto.Trend,
            Title = dto.Title,
            Description = dto.Description ?? string.Empty,
            ImageUrl = imageUrl ?? string.Empty,
            IsPublic = string.Equals(dto.Status, "public", StringComparison.OrdinalIgnoreCase),
            Tags = dto.Tags != null ? string.Join(',', dto.Tags) : string.Empty
        };

        await _uow.BeginTransactionAsync();
        await _ideas.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result<IdeaDto>.Success(ToDto(entity));
    }

    public async Task<Result<IdeaDto>> UpdateAsync(int id, UpdateIdeaDto dto)
    {
        var entity = await _ideas.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return Result<IdeaDto>.NotFound("ایده یافت نشد");

        if (!ApplicationLayer.Common.Enums.TimeframeUnit.IsValid(dto.Timeframe))
            return Result<IdeaDto>.ValidationFailure("محدوده زمانی نامعتبر است");
        if (!ApplicationLayer.Common.Enums.IdeaTrend.IsValid(dto.Trend))
            return Result<IdeaDto>.ValidationFailure("روند نامعتبر است");
        if (!ApplicationLayer.Common.Enums.IdeaVisibility.IsValid(dto.Status))
            return Result<IdeaDto>.ValidationFailure("وضعیت نامعتبر است");

        var crypto = await _cryptos.GetDbSet().FirstOrDefaultAsync(x => x.Symbol == dto.Symbol);
        if (crypto == null)
            return Result<IdeaDto>.NotFound("نماد یافت نشد");

        entity.Symbol = dto.Symbol;
        entity.CryptocurrencyId = crypto.Id;
        entity.Timeframe = dto.Timeframe;
        entity.Trend = dto.Trend;
        entity.Title = dto.Title;
        entity.Description = dto.Description ?? string.Empty;
        if (dto.Image != null)
        {
            var saveImage = await _fileStorage.SaveIdeaImageAsync(dto.Image);
            if (saveImage.IsFailure)
                return Result<IdeaDto>.Failure(saveImage.Error);
            entity.ImageUrl = saveImage.Value;
        }
        entity.IsPublic = string.Equals(dto.Status, "public", StringComparison.OrdinalIgnoreCase);
        entity.Tags = dto.Tags != null ? string.Join(',', dto.Tags) : string.Empty;
        entity.IsActive = dto.IsActive;
        entity.MarkAsUpdated();

        await _uow.BeginTransactionAsync();
        await _ideas.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        return Result<IdeaDto>.Success(ToDto(entity));
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var entity = await _ideas.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return Result.NotFound("ایده یافت نشد");

        await _uow.BeginTransactionAsync();
        _ideas.Remove(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    public async Task<Result<IdeaDto>> GetByIdAsync(int id)
    {
        var entity = await _ideas.GetDbSet().FirstOrDefaultAsync(x => x.Id == id && x.IsPublic);
        if (entity == null)
            return Result<IdeaDto>.NotFound("ایده یافت نشد");
        return Result<IdeaDto>.Success(ToDto(entity));
    }

    public async Task<Result<IdeasPageDto>> GetPublicAsync(GetIdeasRequestDto dto)
    {
        var query = _ideas.Query().Where(x => x.IsPublic);

        if (!string.IsNullOrWhiteSpace(dto.Symbol))
            query = query.Where(x => x.Symbol == dto.Symbol);
        if (!string.IsNullOrWhiteSpace(dto.Timeframe))
            query = query.Where(x => x.Timeframe == dto.Timeframe);
        if (!string.IsNullOrWhiteSpace(dto.Trend))
            query = query.Where(x => x.Trend == dto.Trend);
        if (dto.Tags != null && dto.Tags.Count > 0)
        {
            foreach (var tag in dto.Tags)
                query = query.Where(x => x.Tags.Contains(tag));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(dto.Pagination.Skip)
            .Take(dto.Pagination.PageSize)
            .Select(x => ToDto(x))
            .ToListAsync();

        var page = new IdeasPageDto
        {
            Items = items,
            Total = total,
            Page = dto.Pagination.Page,
            PageSize = dto.Pagination.PageSize
        };
        return Result<IdeasPageDto>.Success(page);
    }

    private static IdeaDto ToDto(Idea entity)
    {
        return new IdeaDto
        {
            Id = entity.Id,
            Symbol = entity.Symbol,
            Timeframe = entity.Timeframe,
            Trend = entity.Trend,
            Title = entity.Title,
            Description = entity.Description,
            Image = entity.ImageUrl,
            Status = entity.IsPublic ? "public" : "private",
            Tags = string.IsNullOrEmpty(entity.Tags) ? new List<string>() : entity.Tags.Split(',').ToList(),
            CreatedAt = entity.CreatedAt
        };
    }
}