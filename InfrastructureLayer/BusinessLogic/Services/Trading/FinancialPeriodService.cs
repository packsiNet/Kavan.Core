using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.FinancialPeriod;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using DomainLayer.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Trading;

[InjectAsScoped]
public class FinancialPeriodService(
    IRepository<FinancialPeriod> _repository,
    IUserContextService _userContext,
    IUnitOfWork _uow
) : IFinancialPeriodService
{
    public async Task<Result<FinancialPeriodDto>> CreateAsync(CreateFinancialPeriodDto dto)
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<FinancialPeriodDto>.Failure(Error.Authentication("User not authenticated"));

        var hasActive = await _repository.Query()
            .AnyAsync(x => x.UserAccountId == userId && !x.IsClosed);

        if (hasActive)
            return Result<FinancialPeriodDto>.Failure(Error.Duplicate("Active financial period already exists"));

        var entity = new FinancialPeriod
        {
            UserAccountId = userId.Value,
            StartDateUtc = dto.StartDateUtc,
            EndDateUtc = dto.EndDateUtc,
            PeriodType = dto.PeriodType,
            IsClosed = false
        };

        await _repository.AddAsync(entity);
        await _uow.SaveChangesAsync();

        return Result<FinancialPeriodDto>.Success(MapToDto(entity));
    }

    public async Task<Result<FinancialPeriodDto>> GetActivePeriodAsync()
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<FinancialPeriodDto>.Failure(Error.Authentication("User not authenticated"));

        var entity = await _repository.Query()
            .FirstOrDefaultAsync(x => x.UserAccountId == userId && !x.IsClosed);

        if (entity == null)
            return Result<FinancialPeriodDto>.Failure(Error.NotFound("No active financial period found"));

        return Result<FinancialPeriodDto>.Success(MapToDto(entity));
    }

    public async Task<Result<FinancialPeriodDto>> ClosePeriodAsync(int id)
    {
        var userId = _userContext.UserId;
        if (userId == null) return Result<FinancialPeriodDto>.Failure(Error.Authentication("User not authenticated"));

        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return Result<FinancialPeriodDto>.Failure(Error.NotFound("Financial period not found"));

        if (entity.UserAccountId != userId)
            return Result<FinancialPeriodDto>.Failure(Error.AccessDenied("Not authorized to access this period"));

        if (entity.IsClosed)
            return Result<FinancialPeriodDto>.Failure(Error.Validation("Period is already closed"));

        entity.IsClosed = true;
        await _repository.UpdateAsync(entity);
        await _uow.SaveChangesAsync();

        return Result<FinancialPeriodDto>.Success(MapToDto(entity));
    }

    private static FinancialPeriodDto MapToDto(FinancialPeriod entity)
    {
        return new FinancialPeriodDto
        {
            Id = entity.Id,
            UserId = entity.UserAccountId,
            StartDateUtc = entity.StartDateUtc,
            EndDateUtc = entity.EndDateUtc,
            PeriodType = entity.PeriodType,
            PeriodTypeName = FinancialPeriodType.FromValue(entity.PeriodType).Name,
            IsClosed = entity.IsClosed,
            CreatedAt = entity.CreatedAt
        };
    }
}
