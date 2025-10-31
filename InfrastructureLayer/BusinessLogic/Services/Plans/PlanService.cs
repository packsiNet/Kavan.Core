using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Plans;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.Plans;

[InjectAsScoped]
public class PlanService(IUnitOfWork _uow, IMapper _mapper,
                         IRepository<Plan> _plans,
                         IRepository<PlanFeature> _features) : IPlanService
{
    public async Task<Result<PlanDto>> CreateAsync(CreatePlanDto dto)
    {
        // Unique Code check
        var exists = await _plans.AnyAsync(p => p.Code == dto.Code);
        if (exists)
            return Result<PlanDto>.DuplicateFailure("کد پلن تکراری است");

        var entity = _mapper.Map<Plan>(dto);
        entity.Features = _mapper.Map<List<PlanFeature>>(dto.Features);

        await _uow.BeginTransactionAsync();
        await _plans.AddAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        var created = await _plans.GetDbSet()
            .Include(x => x.Features)
            .FirstAsync(x => x.Id == entity.Id);
        var resultDto = _mapper.Map<PlanDto>(created);
        return Result<PlanDto>.Success(resultDto);
    }

    public async Task<Result<PlanDto>> UpdateAsync(int id, UpdatePlanDto dto)
    {
        var entity = await _plans.GetDbSet()
            .Include(x => x.Features)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return Result<PlanDto>.NotFound("پلن یافت نشد");

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.PriceMonthly = dto.PriceMonthly;
        entity.PriceYearly = dto.PriceYearly;
        entity.IsPublic = dto.IsPublic;
        entity.IsActive = dto.IsActive;
        entity.MarkAsUpdated();

        // sync features: simplistic replace approach
        var existingFeatures = entity.Features.ToList();
        foreach (var ef in existingFeatures)
            _features.Remove(ef);
        entity.Features = _mapper.Map<List<PlanFeature>>(dto.Features);

        await _uow.BeginTransactionAsync();
        await _plans.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();

        var updated = await _plans.GetDbSet()
            .Include(x => x.Features)
            .FirstAsync(x => x.Id == id);
        var resultDto = _mapper.Map<PlanDto>(updated);
        return Result<PlanDto>.Success(resultDto);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var entity = await _plans.GetDbSet().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return Result.NotFound("پلن یافت نشد");

        await _uow.BeginTransactionAsync();
        _plans.Remove(entity);
        await _uow.SaveChangesAsync();
        await _uow.CommitAsync();
        return Result.Success();
    }

    public async Task<Result<PlanDto>> GetByIdAsync(int id)
    {
        var entity = await _plans.GetDbSet()
            .Include(x => x.Features)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return Result<PlanDto>.NotFound("پلن یافت نشد");
        return Result<PlanDto>.Success(_mapper.Map<PlanDto>(entity));
    }

    public async Task<Result<List<PlanDto>>> GetAllAsync()
    {
        var list = await _plans.GetDbSet()
            .Include(x => x.Features)
            .OrderBy(x => x.PriceMonthly)
            .ToListAsync();
        var dto = _mapper.Map<List<PlanDto>>(list);
        return Result<List<PlanDto>>.Success(dto);
    }
}